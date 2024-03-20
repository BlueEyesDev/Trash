Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Security.Principal
Imports System.Text
Imports System.Threading
Imports Microsoft.Win32

#If NET6_0 Then
Imports WbemScripting
Imports System.Text.Json
#Else
Imports System.Web.Script.Serialization
Imports System.Management
#End If


Friend Class BlueLicense

	Public Shared Property ProductID As String
	Public Shared Property ProductPublicKey As String
	Private Shared Property Incompatibility As String

	Public Shared Sub Initialize(productID As String, productPublicKey As String)
		BlueLicense.ProductID = productID
		BlueLicense.ProductPublicKey = productPublicKey
		Try
			Dim result As String = BlueLicense.HTTP("http://bluelicense.local/api/" + BlueLicense.ProductID + "/checksum", "GET", Nothing)
			If BlueLicense.Checksum(Process.GetCurrentProcess().Modules(0).FileName, SHA512.Create()).Item1 <> result AndAlso result <> "0" Then
				Throw New ArgumentException("The file is corrupted or damaged.")
			End If
		Catch
			Throw New InvalidOperationException("Software verification failed")
		End Try
		'If BlueLicense.Protect.ThreadState <> System.Threading.ThreadState.Running Then
		BlueLicense.Protect.Start()
		'End If
	End Sub

	Shared Sub New()
#If Not DEBUG Then
		If Not New WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator) Then
			Try
				Process.Start(New ProcessStartInfo() With {.UseShellExecute = True, .WorkingDirectory = Environment.CurrentDirectory, .FileName = Process.GetCurrentProcess().MainModule.FileName, .Verb = "runas", .Arguments = String.Join(" ", Environment.GetCommandLineArgs())})
			Catch
			End Try
			Process.GetCurrentProcess().Kill()
		End If
#End If

	End Sub
	Public Shared Function GetVariable(key As String) As String
		Dim Value As String = String.Empty
		If BlueLicense._variable.TryGetValue(key, Value) Then
			Return Value
		End If
		Throw New KeyNotFoundException("The key '" + key + "' was not found.")
	End Function

	Public Shared Function Login(User As String, Password As String) As BlueLicense.LoginResult
		Dim result As BlueLicense.LoginResult
		Try
			Dim directory As Dictionary(Of String, String) = New Dictionary(Of String, String)() From {{"User", User}, {"Password", BlueLicense.Hash(BlueLicense.ProductID + BlueLicense.ProductPublicKey + Password, SHA512.Create()).Item1}, {"Hwid", BlueLicense.GetHwid()}}

#If NET6_0 Then
            Dim jsonString As New StringContent($"[""{TDESEncryption(JsonSerializer.Serialize(directory))}""]", Encoding.UTF8, "application/json")
#Else
			Dim jsonString As New StringContent($"[""{TDESEncryption(New JavaScriptSerializer().Serialize(directory))}""]", Encoding.UTF8, "application/json")
#End If
			Dim responseBody As String = HTTP($"http://bluelicense.local/api/{ProductID}/login", "POST", jsonString)
#If NET6_0 Then
            Dim json As Dictionary(Of String, Object) = JsonSerializer.Deserialize(Of Dictionary(Of String, Object))(responseBody)
#Else
			Dim json As Dictionary(Of String, Object) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(responseBody)
#End If

			For Each obj As Object In [Enum].GetValues(GetType(BlueLicense.LoginResult))
				Dim enumValue As BlueLicense.LoginResult = CType(obj, BlueLicense.LoginResult)
				If json("Result").ToString() = BlueLicense.Hash(String.Format("{0}{1}{2}{3}", New Object() {User, directory("Password"), BlueLicense.GetHwid(), enumValue}), SHA512.Create()).Item1 Then
					If enumValue = BlueLicense.LoginResult.Success Then
#If NET6_0 Then
						For Each [property] In CType(json("Variables"), JsonElement).EnumerateObject()
                            _variable.Add([property].Name, [property].Value.GetString())
                        Next
#Else
						For Each kvp As KeyValuePair(Of String, Object) In TryCast(json("Variables"), Dictionary(Of String, Object))
							BlueLicense._variable.Add(kvp.Key, kvp.Value.ToString())
						Next
#End If
					End If
					Return enumValue
				End If
			Next
			result = BlueLicense.LoginResult.[Error]
		Catch ex As Exception
			Throw New HttpRequestException("Request failed with status code: 500")
		End Try
		Return result
	End Function
	Public Shared Function Register(User As String, Password As String, SerialKey As String) As BlueLicense.RegisterResult
		Dim result As BlueLicense.RegisterResult
		Try
			Dim directory As Dictionary(Of String, String) = New Dictionary(Of String, String)() From {{"User", User}, {"Password", BlueLicense.Hash(BlueLicense.ProductID + BlueLicense.ProductPublicKey + Password, SHA512.Create()).Item1}, {"SerialKey", SerialKey}, {"Hwid", BlueLicense.GetHwid()}}
#If NET6_0 Then
            Dim jsonString As New StringContent($"[""{TDESEncryption(JsonSerializer.Serialize(directory))}""]", Encoding.UTF8, "application/json")
#Else
			Dim jsonString As New StringContent($"[""{TDESEncryption(New JavaScriptSerializer().Serialize(directory))}""]", Encoding.UTF8, "application/json")
#End If
			Dim responseBody As String = HTTP($"http://bluelicense.local/api/{ProductID}/register", "POST", jsonString)
#If NET6_0 Then
            Dim json As Dictionary(Of String, String) = JsonSerializer.Deserialize(Of Dictionary(Of String, String))(responseBody)
#Else
			Dim json As Dictionary(Of String, String) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, String))(responseBody)
#End If
			For Each obj As Object In [Enum].GetValues(GetType(BlueLicense.RegisterResult))
				Dim enumValue As BlueLicense.RegisterResult = CType(obj, BlueLicense.RegisterResult)
				If json("Result").ToString() = BlueLicense.Hash(String.Format("{0}{1}{2}{3}", New Object() {User, directory("Password"), BlueLicense.GetHwid(), enumValue}), SHA512.Create()).Item1 Then
					Return enumValue
				End If
			Next
			result = BlueLicense.RegisterResult.[Error]
		Catch ex As Exception
			Throw New HttpRequestException("Request failed with status code: 500")
		End Try
		Return result
	End Function
	Private Shared Function HTTP(Url As String, Method As String, Optional Data As StringContent = Nothing) As String
		Dim result As String
		Using client As HttpClient = New HttpClient()
			Dim response As HttpResponseMessage
			If Method = "POST" Then
				If Data Is Nothing Then
					Throw New ArgumentException("Invalid data provided.")
				End If
				response = client.PostAsync(Url, Data).Result
			Else
				response = client.GetAsync(Url).Result
			End If
			If Not response IsNot Nothing AndAlso response.IsSuccessStatusCode Then
				Throw New HttpRequestException(String.Format("Request failed with status code: {0}", response.StatusCode))
			End If
			result = BlueLicense.TDESDecrypter(response.Content.ReadAsStringAsync().Result)
		End Using
		Return result
	End Function
	Private Shared Function TDESEncryption(Input As String) As String
		Dim result As String
		Using tripleDES As TripleDES = TripleDES.Create()
			tripleDES.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(BlueLicense.ProductPublicKey))
			tripleDES.Mode = CipherMode.ECB
			tripleDES.Padding = PaddingMode.PKCS7
			Using encryptor As ICryptoTransform = tripleDES.CreateEncryptor()
				Dim plainBytes As Byte() = Encoding.UTF8.GetBytes(Input)
				result = Convert.ToBase64String(encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length))
			End Using
		End Using
		Return result
	End Function
	Private Shared Function TDESDecrypter(Input As String) As String
		Dim [string] As String
		Using tripleDES As TripleDES = TripleDES.Create()
			tripleDES.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(BlueLicense.ProductPublicKey))
			tripleDES.Mode = CipherMode.ECB
			tripleDES.Padding = PaddingMode.PKCS7
			Dim encryptedBytes As Byte() = Convert.FromBase64String(Input)
			Using decryptor As ICryptoTransform = tripleDES.CreateDecryptor()
				Dim decryptedBytes As Byte() = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length)
				[string] = Encoding.UTF8.GetString(decryptedBytes)
			End Using
		End Using
		Return [string]
	End Function
	Public Shared Function Checksum(path As String, hash As HashAlgorithm) As (Hexa As String, Binary As Byte())
		Dim result As ValueTuple(Of String, Byte())
		Using Data As FileStream = File.OpenRead(path)
			Dim ComputeHash As Byte() = hash.ComputeHash(Data)
			Dim hexa As String = BitConverter.ToString(ComputeHash).Replace("-", String.Empty).ToLower()
			result = New ValueTuple(Of String, Byte())(hexa, ComputeHash)
		End Using
		Return result
	End Function
	Public Shared Function Hash(Data As String, HashAlgo As HashAlgorithm) As (Hexa As String, Binary As Byte())
		Dim ComputeHash As Byte() = HashAlgo.ComputeHash(Encoding.UTF8.GetBytes(Data))
		Dim hexa As String = BitConverter.ToString(ComputeHash).Replace("-", String.Empty).ToLower()
		Return New ValueTuple(Of String, Byte())(hexa, ComputeHash)
	End Function
	Private Shared Function GetHwid() As String
		Return WindowsIdentity.GetCurrent().User.Value.Replace("-", "")
	End Function
	Private Shared Sub Security()
		While True
#If Not DEBUG Then
            CheckHostsFile()
            DisableDNSServers()
            DisableProxy()
#End If
			If GetProcess() Then
				Throw New ExternalException($"Incompatibility detected between {Incompatibility}.")
			End If

			Thread.Sleep(5)
		End While
	End Sub
	Private Shared Function GetProcess() As Boolean
		Dim processes As Process() = Process.GetProcesses()
		Dim i As Integer = 0
		While i < processes.Length
			Dim item As Process = processes(i)
			BlueLicense.Incompatibility = item.ProcessName
			Dim result As Boolean
			If BlueLicense.IsCrackingTool(item.ProcessName.ToLower()) Then
				result = True
			Else
				If Not BlueLicense.IsCrackingTool(item.MainWindowTitle.ToLower()) Then
					i += 1
					Continue While
				End If
				result = True
			End If
			Return result
		End While
		Return False
	End Function
	Private Shared Function IsCrackingTool(processName As String) As Boolean
		For Each item As String In New String() {"fiddler", "wireshark", "charles", "snpa", "dumcap", "dnspy", "megadumper", "cheatengine", "cheat engine"}
			If processName.Contains(item) Then
				Return True
			End If
		Next
		Return False
	End Function
	Private Shared Sub CheckHostsFile()
		Dim hostsFilePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\etc\hosts")
		If File.Exists(hostsFilePath) Then
			Dim hostsContent As String = File.ReadAllText(hostsFilePath)
			If hostsContent.ToLower().Contains("bluelicence.com") Then
				File.WriteAllText(hostsFilePath, "")
			End If
		End If
	End Sub
	Private Shared Sub DisableDNSServers()
		Try
#If NET6_0 Then
			For Each adapter As SWbemObject In New SWbemLocator().ConnectServer(".", "root\\cimv2", "", "", "", "", 0, Nothing).ExecQuery("SELECT * FROM Win32_NetworkAdapterConfiguration")
                If DirectCast(adapter.Properties_.Item("IPEnabled").get_Value(), Boolean) Then
                    Dim method = adapter.Methods_.Item("SetDNSServerSearchOrder").InParameters.SpawnInstance_()
                    method.Properties_.Item("DNSServerSearchOrder").set_Value(Nothing)
                    Dim result = adapter.ExecMethod_("SetDNSServerSearchOrder", method)
                    Dim resultCode = DirectCast(result.Properties_.Item("ReturnValue").get_Value(), UInteger)
                    If resultCode <> 0 Then
                        Throw New ArgumentException($"Failed to update DNS server search order. Return code: {resultCode}")
                    End If
                End If
            Next
#Else
			Dim manage As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")
			For Each managementBaseObject As ManagementBaseObject In manage.GetInstances()
				Dim manageobjet As ManagementObject = CType(managementBaseObject, ManagementObject)
				If Convert.ToBoolean(manageobjet("IPEnabled")) Then
					Dim manageobjetbase As ManagementBaseObject = manageobjet.GetMethodParameters("SetDNSServerSearchOrder")
					If manageobjetbase IsNot Nothing Then
						manageobjetbase("DNSServerSearchOrder") = Nothing
						manageobjet.InvokeMethod("SetDNSServerSearchOrder", manageobjetbase, Nothing)
					End If
				End If
			Next
#End If
		Catch ex As Exception
			Throw New ArgumentException("An error occurred: " + ex.Message)
		End Try
	End Sub
	Private Shared Sub DisableProxy()
		Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Internet Settings", True).SetValue("ProxyEnable", 0)
	End Sub
	Private Shared Protect As New Thread(New ThreadStart(AddressOf Security)) With {.IsBackground = True}
	Private Shared _variable As Dictionary(Of String, String) = New Dictionary(Of String, String)()
	Public Enum LoginResult
		Success
		Fail
		Banned
		Subscription
		[Error]
	End Enum
	Public Enum RegisterResult
		Success
		Fail
		Banned
		InvalidKey
		AlreadyRegistered
		[Error]
	End Enum
End Class

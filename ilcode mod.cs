//code C#

if (!File.Exists("TwitchMod.txt"))
	File.Create("TwitchMod.txt");
string[] array = File.ReadAllLines("TwitchMod.txt");
if (array.Length != 0) {
	string text = array[0];
	string[] array2 = new string[array.Length - 1];
	Array.Copy(array, 1, array2, 0, array2.Length);
	File.WriteAllLines("TwitchMod.txt", array2);
	string[] array3 = text.Split('|');
	if (array3[1] == "item") {
		ErrorMessage.AddDebug($"The Viewer : {array3[0]} add {array3[2]} in your inventory");
		DevConsole.SendConsoleCommand(array3[1] + " " + array3[2]);
		return;
	}
	if (array3[1] == "spawn") {
		ErrorMessage.AddDebug($"The Viewer : {array3[0]} Spawn {array3[2]} enemy for you ! good luck !");
		DevConsole.SendConsoleCommand(array3[1] + " " + array3[2]);
		return;
	}
	if (array3[1] == "malus") {
		if (array3[2] == "horde") {
			ErrorMessage.AddDebug($"The Viewer : {array3[0]}  start HORDE ! good luck !");
			for (int i = 0; i < 25; i++) {
					DevConsole.SendConsoleCommand("spawn squidshark");
					DevConsole.SendConsoleCommand("spawn chelicerate");
					DevConsole.SendConsoleCommand("spawn shadowleviathan");
					DevConsole.SendConsoleCommand("spawn cryptosuchus");
			}
			return;
		}
		if (array3[2] == "kill") {
			DevConsole.SendConsoleCommand("spawn beacon");
			ErrorMessage.AddDebug($"The Viewer : {array3[0]}  gives you a penalty {array3[2]} ! Bad Viewer !!!");
			DevConsole.SendConsoleCommand("kill");
			return;
		}
		DevConsole.SendConsoleCommand(array3[2]);
		ErrorMessage.AddDebug($"The Viewer : {array3[0]}  gives you a penalty {array3[2]} ! Bad Viewer !!!");
			return;
	} else if (array3[1] == "bonus") {
		if (array3[2] == "charge")
			DevConsole.SendConsoleCommand(array3[2] + " 100");
		else
			DevConsole.SendConsoleCommand(array3[2]);
		
		ErrorMessage.AddDebug($"The Viewer : {array3[0]} gives you a Bonus {array3[2]} !  Best Viewer !!!");
	}
}


//ILCODE

IL_017D: ldstr     "TwitchMod.txt"
IL_0182: call      bool [mscorlib]System.IO.File::Exists(string)
IL_0187: brtrue.s  IL_0194

IL_0189: ldstr     "TwitchMod.txt"
IL_018E: call      class [mscorlib]System.IO.FileStream [mscorlib]System.IO.File::Create(string)
IL_0193: pop

IL_0194: ldstr     "TwitchMod.txt"
IL_0199: call      string[] [mscorlib]System.IO.File::ReadAllLines(string)
IL_019E: stloc.0
IL_019F: ldloc.0
IL_01A0: ldlen
IL_01A1: brfalse   IL_0436

IL_01A6: ldloc.0
IL_01A7: ldc.i4.0
IL_01A8: ldelem.ref
IL_01A9: stloc.2
IL_01AA: ldloc.0
IL_01AB: ldlen
IL_01AC: conv.i4
IL_01AD: ldc.i4.1
IL_01AE: sub
IL_01AF: newarr    [mscorlib]System.String
IL_01B4: stloc.3
IL_01B5: ldloc.0
IL_01B6: ldc.i4.1
IL_01B7: ldloc.3
IL_01B8: ldc.i4.0
IL_01B9: ldloc.3
IL_01BA: ldlen
IL_01BB: conv.i4
IL_01BC: call      void [mscorlib]System.Array::Copy(class [mscorlib]System.Array, int32, class [mscorlib]System.Array, int32, int32)
IL_01C1: ldstr     "TwitchMod.txt"
IL_01C6: ldloc.3
IL_01C7: call      void [mscorlib]System.IO.File::WriteAllLines(string, string[])
IL_01CC: ldloc.2
IL_01CD: ldc.i4.1
IL_01CE: newarr    [mscorlib]System.Char
IL_01D3: dup
IL_01D4: ldc.i4.0
IL_01D5: ldc.i4.s  124
IL_01D7: stelem.i2
IL_01D8: callvirt  instance string[] [mscorlib]System.String::Split(char[])
IL_01DD: stloc.s   V_4
IL_01DF: ldloc.s   V_4
IL_01E1: ldc.i4.1
IL_01E2: ldelem.ref
IL_01E3: ldstr     "item"
IL_01E8: call      bool [mscorlib]System.String::op_Equality(string, string)
IL_01ED: brfalse.s IL_023D

IL_01EF: ldc.i4.5
IL_01F0: newarr    [mscorlib]System.String
IL_01F5: dup
IL_01F6: ldc.i4.0
IL_01F7: ldstr     "The Viewer : "
IL_01FC: stelem.ref
IL_01FD: dup
IL_01FE: ldc.i4.1
IL_01FF: ldloc.s   V_4
IL_0201: ldc.i4.0
IL_0202: ldelem.ref
IL_0203: stelem.ref
IL_0204: dup
IL_0205: ldc.i4.2
IL_0206: ldstr     " add "
IL_020B: stelem.ref
IL_020C: dup
IL_020D: ldc.i4.3
IL_020E: ldloc.s   V_4
IL_0210: ldc.i4.2
IL_0211: ldelem.ref
IL_0212: stelem.ref
IL_0213: dup
IL_0214: ldc.i4.4
IL_0215: ldstr     " in your inventory"
IL_021A: stelem.ref
IL_021B: call      string [mscorlib]System.String::Concat(string[])
IL_0220: call      void ErrorMessage::AddDebug(string)
IL_0225: ldloc.s   V_4
IL_0227: ldc.i4.1
IL_0228: ldelem.ref
IL_0229: ldstr     " "
IL_022E: ldloc.s   V_4
IL_0230: ldc.i4.2
IL_0231: ldelem.ref
IL_0232: call      string [mscorlib]System.String::Concat(string, string, string)
IL_0237: call      void DevConsole::SendConsoleCommand(string)
IL_023C: ret

IL_023D: ldloc.s   V_4
IL_023F: ldc.i4.1
IL_0240: ldelem.ref
IL_0241: ldstr     "spawn"
IL_0246: call      bool [mscorlib]System.String::op_Equality(string, string)
IL_024B: brfalse.s IL_029B

IL_024D: ldc.i4.5
IL_024E: newarr    [mscorlib]System.String
IL_0253: dup
IL_0254: ldc.i4.0
IL_0255: ldstr     "The Viewer : "
IL_025A: stelem.ref
IL_025B: dup
IL_025C: ldc.i4.1
IL_025D: ldloc.s   V_4
IL_025F: ldc.i4.0
IL_0260: ldelem.ref
IL_0261: stelem.ref
IL_0262: dup
IL_0263: ldc.i4.2
IL_0264: ldstr     " Spawn "
IL_0269: stelem.ref
IL_026A: dup
IL_026B: ldc.i4.3
IL_026C: ldloc.s   V_4
IL_026E: ldc.i4.2
IL_026F: ldelem.ref
IL_0270: stelem.ref
IL_0271: dup
IL_0272: ldc.i4.4
IL_0273: ldstr     " enemy for you ! good luck !"
IL_0278: stelem.ref
IL_0279: call      string [mscorlib]System.String::Concat(string[])
IL_027E: call      void ErrorMessage::AddDebug(string)
IL_0283: ldloc.s   V_4
IL_0285: ldc.i4.1
IL_0286: ldelem.ref
IL_0287: ldstr     " "
IL_028C: ldloc.s   V_4
IL_028E: ldc.i4.2
IL_028F: ldelem.ref
IL_0290: call      string [mscorlib]System.String::Concat(string, string, string)
IL_0295: call      void DevConsole::SendConsoleCommand(string)
IL_029A: ret

IL_029B: ldloc.s   V_4
IL_029D: ldc.i4.1
IL_029E: ldelem.ref
IL_029F: ldstr     "malus"
IL_02A4: call      bool [mscorlib]System.String::op_Equality(string, string)
IL_02A9: brfalse   IL_03C2

IL_02AE: ldloc.s   V_4
IL_02B0: ldc.i4.2
IL_02B1: ldelem.ref
IL_02B2: ldstr     "horde"
IL_02B7: call      bool [mscorlib]System.String::op_Equality(string, string)
IL_02BC: brfalse.s IL_0327

IL_02BE: ldc.i4.4
IL_02BF: newarr    [mscorlib]System.String
IL_02C4: dup
IL_02C5: ldc.i4.0
IL_02C6: ldstr     "The Viewer : "
IL_02CB: stelem.ref
IL_02CC: dup
IL_02CD: ldc.i4.1
IL_02CE: ldloc.s   V_4
IL_02D0: ldc.i4.0
IL_02D1: ldelem.ref
IL_02D2: stelem.ref
IL_02D3: dup
IL_02D4: ldc.i4.2
IL_02D5: ldstr     " start HORDE "
IL_02DA: stelem.ref
IL_02DB: dup
IL_02DC: ldc.i4.3
IL_02DD: ldstr     " Good LUCK !!!!"
IL_02E2: stelem.ref
IL_02E3: call      string [mscorlib]System.String::Concat(string[])
IL_02E8: call      void ErrorMessage::AddDebug(string)
IL_02ED: ldc.i4.0
IL_02EE: stloc.s   V_5
IL_02F0: br.s      IL_0320
	// loop start (head: IL_0320)
	IL_02F2: ldstr     "spawn squidshark"
	IL_02F7: call      void DevConsole::SendConsoleCommand(string)
	IL_02FC: ldstr     "spawn chelicerate"
	IL_0301: call      void DevConsole::SendConsoleCommand(string)
	IL_0306: ldstr     "spawn shadowleviathan"
	IL_030B: call      void DevConsole::SendConsoleCommand(string)
	IL_0310: ldstr     "spawn cryptosuchus"
	IL_0315: call      void DevConsole::SendConsoleCommand(string)
	IL_031A: ldloc.s   V_5
	IL_031C: ldc.i4.1
	IL_031D: add
	IL_031E: stloc.s   V_5

	IL_0320: ldloc.s   V_5
	IL_0322: ldc.i4.s  25
	IL_0324: blt.s     IL_02F2
	// end loop

IL_0326: ret

IL_0327: ldloc.s   V_4
IL_0329: ldc.i4.2
IL_032A: ldelem.ref
IL_032B: ldstr     "kill"
IL_0330: call      bool [mscorlib]System.String::op_Equality(string, string)
IL_0335: brfalse.s IL_0382

IL_0337: ldstr     "spawn beacon"
IL_033C: call      void DevConsole::SendConsoleCommand(string)
IL_0341: ldc.i4.5
IL_0342: newarr    [mscorlib]System.String
IL_0347: dup
IL_0348: ldc.i4.0
IL_0349: ldstr     "The Viewer : "
IL_034E: stelem.ref
IL_034F: dup
IL_0350: ldc.i4.1
IL_0351: ldloc.s   V_4
IL_0353: ldc.i4.0
IL_0354: ldelem.ref
IL_0355: stelem.ref
IL_0356: dup
IL_0357: ldc.i4.2
IL_0358: ldstr     " gives you a penalty "
IL_035D: stelem.ref
IL_035E: dup
IL_035F: ldc.i4.3
IL_0360: ldloc.s   V_4
IL_0362: ldc.i4.2
IL_0363: ldelem.ref
IL_0364: stelem.ref
IL_0365: dup
IL_0366: ldc.i4.4
IL_0367: ldstr     " Bad Viewer !!!"
IL_036C: stelem.ref
IL_036D: call      string [mscorlib]System.String::Concat(string[])
IL_0372: call      void ErrorMessage::AddDebug(string)
IL_0377: ldstr     "kill"
IL_037C: call      void DevConsole::SendConsoleCommand(string)
IL_0381: ret

IL_0382: ldloc.s   V_4
IL_0384: ldc.i4.2
IL_0385: ldelem.ref
IL_0386: call      void DevConsole::SendConsoleCommand(string)
IL_038B: ldc.i4.5
IL_038C: newarr    [mscorlib]System.String
IL_0391: dup
IL_0392: ldc.i4.0
IL_0393: ldstr     "The Viewer : "
IL_0398: stelem.ref
IL_0399: dup
IL_039A: ldc.i4.1
IL_039B: ldloc.s   V_4
IL_039D: ldc.i4.0
IL_039E: ldelem.ref
IL_039F: stelem.ref
IL_03A0: dup
IL_03A1: ldc.i4.2
IL_03A2: ldstr     " gives you a penalty "
IL_03A7: stelem.ref
IL_03A8: dup
IL_03A9: ldc.i4.3
IL_03AA: ldloc.s   V_4
IL_03AC: ldc.i4.2
IL_03AD: ldelem.ref
IL_03AE: stelem.ref
IL_03AF: dup
IL_03B0: ldc.i4.4
IL_03B1: ldstr     " Bad Viewer !!!"
IL_03B6: stelem.ref
IL_03B7: call      string [mscorlib]System.String::Concat(string[])
IL_03BC: call      void ErrorMessage::AddDebug(string)
IL_03C1: ret

IL_03C2: ldloc.s   V_4
IL_03C4: ldc.i4.1
IL_03C5: ldelem.ref
IL_03C6: ldstr     "bonus"
IL_03CB: call      bool [mscorlib]System.String::op_Equality(string, string)
IL_03D0: brfalse.s IL_0436

IL_03D2: ldloc.s   V_4
IL_03D4: ldc.i4.2
IL_03D5: ldelem.ref
IL_03D6: ldstr     "charge"
IL_03DB: call      bool [mscorlib]System.String::op_Equality(string, string)
IL_03E0: brfalse.s IL_03F7

IL_03E2: ldloc.s   V_4
IL_03E4: ldc.i4.2
IL_03E5: ldelem.ref
IL_03E6: ldstr     " 100"
IL_03EB: call      string [mscorlib]System.String::Concat(string, string)
IL_03F0: call      void DevConsole::SendConsoleCommand(string)
IL_03F5: br.s      IL_0400

IL_03F7: ldloc.s   V_4
IL_03F9: ldc.i4.2
IL_03FA: ldelem.ref
IL_03FB: call      void DevConsole::SendConsoleCommand(string)

IL_0400: ldc.i4.5
IL_0401: newarr    [mscorlib]System.String
IL_0406: dup
IL_0407: ldc.i4.0
IL_0408: ldstr     "The Viewer : "
IL_040D: stelem.ref
IL_040E: dup
IL_040F: ldc.i4.1
IL_0410: ldloc.s   V_4
IL_0412: ldc.i4.0
IL_0413: ldelem.ref
IL_0414: stelem.ref
IL_0415: dup
IL_0416: ldc.i4.2
IL_0417: ldstr     " gives you a Bonus "
IL_041C: stelem.ref
IL_041D: dup
IL_041E: ldc.i4.3
IL_041F: ldloc.s   V_4
IL_0421: ldc.i4.2
IL_0422: ldelem.ref
IL_0423: stelem.ref
IL_0424: dup
IL_0425: ldc.i4.4
IL_0426: ldstr     " Best Viewer !!!"
IL_042B: stelem.ref
IL_042C: call      string [mscorlib]System.String::Concat(string[])
IL_0431: call      void ErrorMessage::AddDebug(string)
IL_0436: ret

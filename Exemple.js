//unminify
console.log("hello"); console.log("world");//Return hello\r\nworld
//minify
console.log("hello") console.log("world")
//ERROR: Uncaught SyntaxError: Unexpected identifier 'console'


//Regex
let b = 'bonjour, comment tu va guaca_fly ?'
/bonjour/g.exec(b)
//ERROR Uncaught ReferenceError: bonjour is not defined at <anonymous>:2:2

//Replace
let phrase = 'les points-virgules est '
let fin = 'complexe'
phrase += fin
(phrase).replace('complexe', 'simple')
//ERROR Uncaught TypeError: fin is not a function at <anonymous>:3:11

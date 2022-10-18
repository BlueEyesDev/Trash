let phrase = "Sans les points virgule il y a pas d'erreur"
let fin = ' Ou pas'
phrase += fin
(phrase).replace('pas', 'des Possibilité')
//Uncaught TypeError: fin is not a function at <anonymous>:3:11

let phrase = "Sans les points virgule il y a pas d'erreur";
let fin = ' Ou pas ';
phrase += fin;
(phrase).replace('pas', 'des Possibilité');
//"Sans les points virgule il y a des Possibilité d'erreur Ou pas"

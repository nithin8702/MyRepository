// function Greet(name){
//     console.log("Hello " + name);
// }

var Greet = (name) => {
    console.log("Hello " + name);
}

//var Greet = (name) => console.log("Hello " + name);

//Greet("World!");


function Calculate(a = 1, b = 2, c = a + b) {
    console.log(a);
    console.log(b);
    console.log(c);
}
var a, b;
Calculate(a, b, 4);
//Calculate(5, 6,3);



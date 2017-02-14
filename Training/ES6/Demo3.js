// function test(a, ...b) {
//     console.log(a);
//     console.log(b);
// }
// test(1, 2, 3, 4, 5);


function test(a, b, c, d) {
    console.log(a);
    console.log(b);
    console.log(c);
    console.log(d);
}
test(1, ...[2, 3]);
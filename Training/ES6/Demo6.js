class Employee{
    constructor(name){
        console.log("Employee constructor called.")
        this.name = name;
    }
    Display(){
        console.log("Manager name is " + this.name);
    }
    static Greet(){
        console.log("Hello");
    }
}
class Manager extends Employee{
    constructor(name){
        super(name);
        console.log("Employee constructor called.")
        console.log(this.name);
    }
    // constructor(name,a){
    //     super(name);
    //     this.a = a;
    //     console.log(this.a);
    // }

}
var manager = new Manager("Sateesh Mani",5);
manager.Display();

//var emp = new Employee("Nithin");
//emp.Display();

//Employee.Greet();


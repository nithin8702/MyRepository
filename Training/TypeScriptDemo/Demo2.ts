interface IEmployee{
    id:Number,
    name:string,
    Display():string
}

class Employee implements IEmployee{
    id = 1;
    name="Nithin";
    constructor(){
        console.log("Test");
    }
    Display():string{
        return this.name;
    }
}
var em = new Employee();
console.log(em.Display());
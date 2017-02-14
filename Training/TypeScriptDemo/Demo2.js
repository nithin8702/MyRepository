var Employee = (function () {
    function Employee() {
        this.id = 1;
        this.name = "Nithin";
        console.log("Test");
    }
    Employee.prototype.Display = function () {
        return this.name;
    };
    return Employee;
}());
var em = new Employee();
console.log(em.Display());

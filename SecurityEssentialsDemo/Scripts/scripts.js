console.log("test");

$.get("https://jsonplaceholder.typicode.com/todos/1", function (response) {
    console.log(response);
});
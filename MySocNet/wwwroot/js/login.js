let token = document.getElementById('token');
let button = document.getElementById('myButton');

function getCook(cookiename) {
    // Get name followed by anything except a semicolon
    let cookiestring=RegExp(cookiename+"=[^;]+").exec(document.cookie);
    // Return everything after the equal sign, or an empty string if the cookie name not found
    return decodeURIComponent(!!cookiestring ? cookiestring.toString().replace(/^[^=]+./,"") : "");
    }
    //Sample usage
    let cookieValue = getCook('key');
  

button.addEventListener('click', function(){
    token.value = cookieValue;
    console.log(cookieValue);
});
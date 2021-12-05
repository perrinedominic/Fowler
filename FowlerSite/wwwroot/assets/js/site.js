$(document).ready(function () {
    if ($("body").is(".Create-User")) {
        $(".hide-me").css("display", "none");
    }

    let cookie = document.cookie.split('; ').reduce((prev, current) => {
        const [name, ...value] = current.split('=');
        prev[name] = value.join('=');
        return prev;
    }, {});

    if (cookie.UserId !== "") {
        console.log("Logged in");
        $('.login-icon').innerHTML = '';
    }

    console.log(cookie);
});
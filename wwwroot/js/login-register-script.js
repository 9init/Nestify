const signUpButton = document.getElementById("signUp");
const signInButton = document.getElementById("signIn");
const container = document.getElementById("container");
const openHomeSignIn = document.getElementById("openHomeSignIn");
const openHomeSignUp = document.getElementById("openHomeSignUp");

signUpButton.addEventListener("click", () => {
  container.classList.add("right-panel-active");
});

signInButton.addEventListener("click", () => {
  container.classList.remove("right-panel-active");
});

$("#login-form").submit(function (event) {
  event.preventDefault(); // Prevent the form from submitting normally
  const email = $("#email").val();
  const password = $("#password").val();

  $.ajax({
    type: "POST",
    url: "/Enroll/login",
    data: { email: email, password: password },
    success: function (data, textStatus, request) {
      // setTimeout(()=>{
      //   const url = "http://localhost:5141/Home/home?IsLoggedIn=true";
      //   window.open(url, "_self");
      // }, 1000)
      console.log(getCookies());
      const url = "http://localhost:5141/Home/home?IsLoggedIn=true";
      window.open(url, "_self");
    },
    error: function (xhr, status, error) {
      const response = JSON.parse(xhr.responseText);
      showMessage("login-error-msg", response.message, true);
    },
  });
});

var getCookies = function () {
  var pairs = document.cookie.split(";");
  var cookies = {};
  for (var i = 0; i < pairs.length; i++) {
    var pair = pairs[i].split("=");
    cookies[(pair[0] + "").trim()] = unescape(pair.slice(1).join("="));
  }
  return cookies;
};

$("#register-form").submit(function (event) {
  event.preventDefault(); // Prevent the form from submitting normally
  let email = $("#remail").val();
  let password = $("#rpassword").val();
  let name = $("#rname").val();
  console.log({ email: email, password: password, name: name });
  $.ajax({
    type: "POST",
    url: "/Enroll/register",
    data: { email: email, password: password, name: name },
    success: function (response) {
      showMessage("register-error-msg", response.message, false);
      console.log(response);
    },
    error: function (xhr, status, error) {
      var response = JSON.parse(xhr.responseText);
      showMessage("register-error-msg", response.message, true);
    },
  });
});

function showMessage(id, msg, isError) {
  const myDiv = document.getElementById(id);
  if (isError) {
    myDiv.style.color = "red";
  } else {
    myDiv.style.color = "green";
  }
  $("#" + id).text(msg);
}

$("#update_btn").click(() => {
  const first_name = $("#input-first-name").val();
  const last_name = $("#input-last-name").val();
  const city = $("#input-city").val();
  const country = $("#input-country").val();
  const postal_code = $("#input-postal-code").val();
  const description = $("#input-description").val();

  const data = {
    first_name,
    last_name,
    city,
    country,
    postal_code,
    description,
  };

  $.ajax({
    type: "POST",
    url: "/Profile/update",
    data,
    success: function (response) {
      showMessage("update-error-msg", response.message, false);
    },
    error: function (xhr, status, error) {
      const response = JSON.parse(xhr.responseText);
      showMessage("update-error-msg", response.message, true);
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

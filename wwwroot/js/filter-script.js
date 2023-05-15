$("#search-btn").click(() => {
    const data = {
        price: parseInt($(".priceInput").val()) || null,
        bedrooms: parseInt($(".bedroomsInput").val()) || null,
        bathrooms: parseInt($(".bathroomsInput").val()) || null,
        area: parseInt($(".areaInput").val()) || null
    };
    console.log(data)
    postData("http://localhost:5141/Filter/search ", data).then((data) => {
        addDataToSearchResult(data);
    });
});

function addDataToSearchResult(data){
    let body = ""

    data.forEach(element => {
        const li = `
        <li>
            <div class="property-card">
              <figure class="card-banner">
                <a href="#">
                  <img
                    src="${element.image}"
                    alt="Comfortable Apartment"
                    class="w-100"
                  />
                </a>

                <div class="card-badge ${element.type == "RENT" ? "green" : "orange"}">For ${element.type}</div>

                <div class="banner-actions">
                  <button class="banner-actions-btn">
                    <ion-icon name="location"></ion-icon>

                    <address>${element.location}</address>
                  </button>

                  <button class="banner-actions-btn">
                    <ion-icon name="camera"></ion-icon>

                    <span>4</span>
                  </button>

                  <button class="banner-actions-btn">
                    <ion-icon name="film"></ion-icon>

                    <span>2</span>
                  </button>
                </div>
              </figure>

              <div class="card-content">
                <div class="card-price"><strong>£${element.price}</strong>${element.type == "RENT" ? "/Month" : "" }</div>

                <h3 class="h3 card-title">
                  <a href="#">${element.title}</a>
                </h3>

                <p class="card-text">${element.description}</p>
                <ul class="card-list">
                  <li class="card-item">
                    <strong>${element.bedrooms}</strong>

                    <ion-icon name="bed-outline"></ion-icon>

                    <span>Bedrooms</span>
                  </li>

                  <li class="card-item">
                    <strong>${element.bathrooms}</strong>

                    <ion-icon name="man-outline"></ion-icon>

                    <span>Bathrooms</span>
                  </li>

                  <li class="card-item">
                  <strong>${element.area}</strong>

                    <ion-icon name="square-outline"></ion-icon>

                    <span>Meter Square</span>
                  </li>
                </ul>
              </div>

              <div class="card-footer">
                <div class="card-author">
                  <figure class="author-avatar">
                    <img
                      src="${element.author_Image}"
                      alt="Adel Shakal"
                      class="w-100"
                    />
                  </figure>

                  <div>
                    <p class="author-name">
                      <a href="#">${element.author_Name}</a>
                    </p>

                    <p class="author-title">${element.author_Agent}</p>
                  </div>
                </div>

                <div class="card-footer-actions">
                  <button class="card-footer-actions-btn">
                    <ion-icon name="resize-outline"></ion-icon>
                  </button>

                  <button class="card-footer-actions-btn">
                    <ion-icon name="heart-outline"></ion-icon>
                  </button>

                  <button class="card-footer-actions-btn">
                    <ion-icon name="add-circle-outline"></ion-icon>
                  </button>
                </div>
              </div>
            </div>
          </li>
        `
        body += li
    });
    $("#search-result").html(body);
}

// Example POST method implementation:
async function postData(url = "", data = {}) {
    // Default options are marked with *
    const response = await fetch(url, {
      method: "POST", // *GET, POST, PUT, DELETE, etc.
      mode: "cors", // no-cors, *cors, same-origin
      cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
      credentials: "same-origin", // include, *same-origin, omit
      headers: {
        "Content-Type": "application/json",
        // 'Content-Type': 'application/x-www-form-urlencoded',
      },
      redirect: "follow", // manual, *follow, error
      referrerPolicy: "no-referrer", // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
      body: JSON.stringify(data), // body data type must match "Content-Type" header
    });
    return response.json(); // parses JSON response into native JavaScript objects
  }
  
  
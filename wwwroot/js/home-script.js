'use strict';

const elemToggleFunc = function (elem) { elem.classList.toggle("active"); }
const navbar = document.querySelector("[data-navbar]");
const overlay = document.querySelector("[data-overlay]");
const navCloseBtn = document.querySelector("[data-nav-close-btn]");
const navOpenBtn = document.querySelector("[data-nav-open-btn]");
const navbarLinks = document.querySelectorAll("[data-nav-link]");

const navElemArr = [overlay, navCloseBtn, navOpenBtn];
for (const element of navbarLinks) { navElemArr.push(element); }

for (const element of navElemArr) {
  element.addEventListener("click", function () {
    elemToggleFunc(navbar);
    elemToggleFunc(overlay);
  });
}

const header = document.querySelector("[data-header]");

window.addEventListener("scroll", function () {
  window.scrollY >= 400 ? header.classList.add("active")
    : header.classList.remove("active");
}); 

// Quick Access

const searchBtn = document.getElementById('searchBtn');
const favoriteBtn = document.getElementById('favoriteBtn');
const profileBtn = document.getElementById('profileBtn');
const registerBtn = document.getElementById('registerBtn');

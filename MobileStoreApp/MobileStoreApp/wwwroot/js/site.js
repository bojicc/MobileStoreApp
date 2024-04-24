// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    $('#showAllPhones').click(function () {
        $.ajax({
            url: '/Shop/AllPhones',
            type: 'GET',
            success: function (data) {
                $('#allPhonesContainer').html(data);
            },
            error: function () {
                alert('Error.');
            }
        });
    });
});


$(document).ready(function () {
    $('body').on('click', '#phoneList a', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        $('#phoneDetails').load(url + ' #container');
    });
});


window.onscroll = function () { scrollFunction() };

function scrollFunction() {
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        document.getElementById("back-to-top-btn").style.display = "block";
    } else {
        document.getElementById("back-to-top-btn").style.display = "none";
    }
}

function scrollToTop() {
    document.body.scrollTop = 0; // Za Safari
    document.documentElement.scrollTop = 0; // Za ostale pregledače
}






let slideIndex = 0;

function moveSlide(n) {
    const slides = document.querySelectorAll('.slider img');
    if (slideIndex + n < 0) {
        slideIndex = slides.length - 1;
    } else if (slideIndex + n >= slides.length) {
        slideIndex = 0;
    } else {
        slideIndex += n;
    }
    const offset = -slideIndex * 100;
    document.querySelector('.slider').style.transform = `translateX(${offset}%)`;
}

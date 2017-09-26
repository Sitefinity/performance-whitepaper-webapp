// JavaScript source code
jQuery(document).ready(function () {
    $(".dropdown").click(
    function () {
        $('.dropdown-menu', this).toggleClass("q-hidden");
    });
    


    $("#myCarousel").carousel({ interval: 8000 });

    $(".pause-button").click(function(){
        $("#myCarousel").carousel("pause");
        $(".play-button").removeClass("hidden");
        $(".pause-button").addClass("hidden");
        $(".carousel-control").addClass('hidden');
    });

    $(".play-button").click(function () {
        $("#myCarousel").carousel("cycle");
        $(".pause-button").removeClass("hidden");
        $(".play-button").addClass("hidden");
        $(".carousel-control").removeClass('hidden');
    });
});


//$(window).on('resize', function () {
//    $(".dropdown").hover(
//        function () {
//            if ($(window).width() > 800) $('.dropdown-menu', this).fadeIn("fast");
//        },
//        function () {
//            if ($(window).width() > 800) $('.dropdown-menu', this).fadeOut("fast");
//        });
//});
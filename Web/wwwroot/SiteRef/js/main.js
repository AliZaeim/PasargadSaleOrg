(function ($) {
    //$.ajax({
    //    url: "/Login",
    //    type: "GET",
    //    success: function (result) {
    //        $("#div12").html(result);
    //    }
    //});
    //$("#div12").load("/Account/Login");
    $(document).find("#btnReg").click(function () {
        $.ajax({
            url: "/Register",
            type: "GET",
            success: function (result) {
                $("#div12").html(result);
            },
            error: function () {
                alert('error');
            }
        });
    });
})(jQuery);


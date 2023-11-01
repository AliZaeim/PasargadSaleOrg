/*=========================================================================================
    File Name: doughnut.js
    Description: Chartjs simple doughnut chart
    ----------------------------------------------------------------------------------------
    Item Name: Robust - Responsive Admin Theme
    Version: 1.2
    Author: PIXINVENT
    Author URL: http://www.themeforest.net/user/pixinvent
==========================================================================================*/

// Doughnut chart
// ------------------------------
$(window).on("load", function(){

    //Get the context of the Chart canvas element we want to select
    var ctx = $("#simple-doughnut-chart");

    // Chart Options
    var chartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        responsiveAnimationDuration:500,
    };

    // Chart Data
    var chartData = {
        labels: ["تهران", "البرز", "خراسان", "خوزستان", "یزد","گیلان","قزوین"],
        datasets: [{
            label: "My First dataset",
            data: [65, 35, 24, 45, 85,60,40],
            backgroundColor: ["#99B898","#FECEA8","#FF847C","#E84A5F","#2A363B","#fff","#fb1234"],
        }]
    };

    var config = {
        type: 'doughnut',

        // Chart Options
        options : chartOptions,

        data : chartData
    };

    // Create the chart
    var doughnutSimpleChart = new Chart(ctx, config);

});
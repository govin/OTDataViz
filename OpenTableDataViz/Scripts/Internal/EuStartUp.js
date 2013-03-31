var viewModel;
$(document).ready(function () {
	$("#loadCharts").on("click", function () {
		$("#chart").empty();
		viewModel.loadCharts();
	});
	$("#loadRadials").on("click", function () {
		viewModel.loadRadials();
	});
	$("#firstRadial, #secondRadial, #thirdRadial").kendoRadialGauge({
		scale: {
			minorUnit: 25,
			majorUnit: 100,
			startAngle: -30,
			endAngle: 210,
			max: 600,
			labels: {
				position: "outside"
			},
			ranges: [
				{
					from: 0,
					to: 200,
					color: "#31A354"
				}, {
					from: 200,
					to: 400,
					color: "#ff7a00"
				}, {
					from: 400,
					to: 600,
					color: "#c20000"
				}
			]
		}
	});
	

	var options =
	{
		bubbleChartUrl: "api/BubbleChart?region=eu",
		radialChartUrl: "api/RadialChart?region=eu",
		neighborhoodLabel: "Neighborhood",
		backLabel: "<<<     Back",
		metroLabel: "Metro"
	};
	viewModel = new BubbleViewModel(options);
	viewModel.getBubbleData();
	viewModel.loadRadials();
});
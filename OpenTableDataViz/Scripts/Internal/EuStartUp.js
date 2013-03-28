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
			minorUnit: 50,
			majorUnit: 100,
			startAngle: -30,
			endAngle: 210,
			max: 1000,
			labels: {
				position: "outside"
			},
			ranges: [
				{
					from: 0,
					to: 300,
					color: "#31A354"
				}, {
					from: 300,
					to: 700,
					color: "#ff7a00"
				}, {
					from: 700,
					to: 1000,
					color: "#c20000"
				}
			]
		}
	});
	viewModel = new BubbleViewModel("api/BubbleChart?region=eu", "api/RadialChart?region=eu");
	viewModel.getBubbleData();
	viewModel.loadRadials();
	$("#list").on("click", ".location", viewModel.fetchData);
});
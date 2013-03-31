var viewModel;
$(document).ready(function () {
	$("#firstRadial, #secondRadial, #thirdRadial").kendoRadialGauge({
		scale: {
			minorUnit: 10,
			majorUnit: 50,
			startAngle: -30,
			endAngle: 210,
			max: 300,
			labels: {
				position: "outside"
			},
			ranges: [
				{
					from: 0,
					to: 100,
					color: "#31A354"
				}, {
					from: 100,
					to: 200,
					color: "#ff7a00"
				}, {
					from: 200,
					to: 300,
					color: "#c20000"
				}
			]
		}
	});

	var options =
	{
		bubbleChartUrl:"api/BubbleChart?region=asia",
		radialChartUrl:"api/RadialChart?region=asia",
		neighborhoodLabel: "Neighborhood",
		backLabel: "<<<     Back",
		metroLabel: "Metro"
	};  

	viewModel = new BubbleViewModel(options);
	viewModel.getBubbleData();
	viewModel.loadRadials();
});
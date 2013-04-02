var viewModel;
$(document).ready(function () {
	var self = this;
	var options =
	{
		bubbleChartUrl: "api/BubbleChart?region=na",
		radialChartUrl: "api/RadialChart?region=na",
		neighborhoodLabel: "Neighborhood",
		backLabel: "Back",
		metroLabel: "Metro"
	};
	viewModel = new BubbleViewModel(options);
	viewModel.getBubbleData();
	viewModel.loadRadials();
	setInterval(function () { viewModel.getBubbleData(); }, 60000);
	setInterval(function () { viewModel.loadRadials(); }, 60000);
});
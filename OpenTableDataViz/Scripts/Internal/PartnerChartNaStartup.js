var viewModel;
$(document).ready(function () {
	var options =
	{
		bubbleChartUrl: "api/PartnerChart?region=na",
		neighborhoodLabel: "Neighborhood",
		backLabel: "Back",
		metroLabel: "Metro"
	};
	viewModel = new PartnerViewModel(options);
	viewModel.getBubbleData();
	setInterval(function () { viewModel.getBubbleData(); }, 60000);
});
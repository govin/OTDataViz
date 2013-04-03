var viewModel;
$(document).ready(function () {
	var options =
	{
		bubbleChartUrl: "api/PartnerChart?region=eu",
		neighborhoodLabel: "Neighborhood",
		backLabel: "Back",
		metroLabel: "Metro"
	};
	viewModel = new PartnerViewModel(options);
	viewModel.getBubbleData();
	setInterval(function () { viewModel.getBubbleData(); }, 60000);
});
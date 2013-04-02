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
	viewModel.loadRadials();
	//setInterval(function () { viewModel.getBubbleData(); }, 60000);
	//setInterval(function () { viewModel.loadRadials(); }, 60000);
});
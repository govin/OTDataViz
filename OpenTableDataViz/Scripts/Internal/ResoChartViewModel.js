var BubbleViewModel = function (options) {
	var self = this;
	this.displayBackButton = false;
	this.neighborhoodLabel = options.neighborhoodLabel;
	this.metroLabel = options.metroLabel;
	this.backLabel = options.backLabel;
	this.bubbleChartUrl = options.bubbleChartUrl;
	this.radialChartUrl = options.radialChartUrl;
	var metro;
	
	this.loadRadials = function () {
		var url = self.radialChartUrl;
		if (typeof metro !== "undefined" && metro !== null) {
			url += "&metro=" + metro;
		}

		$.getJSON(url, function (data) {
			$("#firstRadialText, #secondRadialText, #thirdRadialText").text("");
			$("#firstRadial, #secondRadial, #thirdRadial").hide();
			if (data) {
				if (data.length > 0) {
					var maxCount = data[0].ReservationCount;
					var closestHundred = (Math.floor(maxCount / 100) + 1) * 100;
					self.initRadials("#firstRadial, #secondRadial, #thirdRadial", closestHundred / 20, closestHundred / 10, closestHundred);
					$("#firstRadial").show();
					$("#firstRadial").data("kendoRadialGauge").value(data[0].ReservationCount);
					$("#firstRadialText").text(data[0].CuisineName);
				}
				if (data.length > 1) {
					$("#secondRadial").show();
					$("#secondRadial").data("kendoRadialGauge").value(data[1].ReservationCount);
					$("#secondRadialText").text(data[1].CuisineName);
				}
				if (data.length > 2) {
					$("#thirdRadial").show();
					$("#thirdRadial").data("kendoRadialGauge").value(data[2].ReservationCount);
					$("#thirdRadialText").text(data[2].CuisineName);
				}
			}
		});
	};

	this.loadCharts = function (restaurants) {
		var locationData = [];
		$("#chart").empty();
		var data = { children: restaurants };

		var diameter = 600,
			format = d3.format(",d"),
			color = d3.scale.category20c();

		var bubble = d3.layout.pack()
			.sort(null)
			.size([diameter, diameter])
			.padding(1.5);

		var svg = d3.select("#chart").append("svg")
			.attr("width", diameter)
			.attr("height", diameter)
			.attr("class", "bubble");

		var node = svg.selectAll(".node")
			.data(bubble.nodes(data)
				.filter(function (d) {
					return !d.children;
				}))
			.enter().append("g")
			.attr("class", "node")
			.attr("transform", function (d) {
				return "translate(" + d.x + "," + d.y + ")";
			});

		node.append("title")
			.text(function (d) {
				return d.className + ": " + format(d.value) + "\n" + d.packageName + "\n" + d.cuisineType;
			});

		var colorCol = [];

		node.append("circle")
			.attr("r", function (d) {
				return d.r;
			})
			.style("fill", function (d) {
				var assignedColor = color(d.packageName);

				var index = $.inArray(assignedColor, colorCol);

				if (index === -1) {
					colorCol.push(assignedColor);
					locationData.push({ location: d.packageName, color: assignedColor, count: d.total, id: d.id });
				}
				return assignedColor;
			});

		node.append("text")
			.attr("dy", ".3em")
			.style("text-anchor", "middle")
			.text(function (d) {
				return d.className.substring(0, d.r / 3);
			});

		d3.select(self.frameElement).style("height", diameter + "px");

		$("#list").empty();

		locationData.sort(self.sortByCount);

		if (self.displayBackButton) {
			var backBtn = $('<div/>', { "class": "backButton" })
				.text(self.backLabel)
				.click(function () {
					metro = null;
					self.displayBackButton = false;
					self.getBubbleData();
					self.loadRadials();
				});
			$("#list").append(backBtn);
			$("#area").text(self.neighborhoodLabel);
			$("#list").off("click", ".location");
			$("#restaurantMetro").text(metro + " Restaurants");
		}
		else {
			$("#restaurantMetro").text("Restaurants");
			$("#list").on("click", ".location", viewModel.fetchData);
			$("#area").text(self.metroLabel);
		}

		$.each(locationData, function (index, loc) {
			var ele = $('<div/>', { "style": "background-color:" + loc.color + ";", "class": "location", "id": "item" + index }).text(loc.location + " - " + loc.count);
			$(ele).data("id", loc.location);
			$("#list").append(ele);
		});
	};

	this.sortByCount = function (a, b) {
		return ((a.count > b.count) ? -1 : ((a.count < b.count) ? 1 : 0));
	};
	
	this.getBubbleData = function () {
		var url = self.bubbleChartUrl;
		var rows = [];
		if (typeof metro !== "undefined" && metro !== null) {
			url += "&metro=" + metro;
		}
		
		$.getJSON(url, function (restaurants) {
			for (var index = 0, length = restaurants.length; index < length; index++) {
				var rest = restaurants[index];
				rows.push({
					packageName: rest.Location,
					className: rest.RName,
					value: rest.ReservationCount,
					total: rest.LocationCount,
					cuisineType: rest.CuisineType
				});
			}
			self.loadCharts(rows);
		});
	};
	
	this.fetchData = function (event) {
		var id = $(event.target).data("id");
		metro = id;
		self.getBubbleData();
		self.loadRadials();
		self.displayBackButton = true;
	};
	
	this.initRadials = function (selector, minorUnit, majorUnit, max) {
		$(selector).kendoRadialGauge({
			scale: {
				minorUnit: minorUnit,
				majorUnit: majorUnit,
				startAngle: -30,
				endAngle: 210,
				max: max,
				labels: {
					position: "outside"
				},
				ranges: [
					{
						from: 0,
						to: max / 3,
						color: "#31A354"
					}, {
						from: max / 3,
						to: (2 * max) / 3,
						color: "#ff7a00"
					}, {
						from: (2 * max) / 3,
						to: max,
						color: "#c20000"
					}
				]
			}
		});
	};
};
var PartnerViewModel = function (options) {
	var self = this;
	this.neighborhoodLabel = options.neighborhoodLabel;
	this.metroLabel = options.metroLabel;
	this.backLabel = options.backLabel;
	this.bubbleChartUrl = options.bubbleChartUrl;
	
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
				return d.className + ": " + format(d.value);
			});

		var partnerNames = [];

		node.append("circle")
			.attr("r", function (d) {
				return d.r;
			})
			.style("fill", function (d) {
				var assignedColor = color(d.packageName);
				
				var index = $.inArray(d.packageName, partnerNames);

				if (index === -1) {
					partnerNames.push(d.packageName);
					locationData.push({ location: d.packageName, color: assignedColor, count: d.value });
				}
				else {
					locationData[index].count = locationData[index].count + d.value;
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
		
		$.each(locationData, function (index, loc) {
			var ele = $('<div/>', { "style": "background-color:" + loc.color + ";", "class": "partnerName" }).text(loc.location + " - " + loc.count);
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
		
		$.getJSON(url, function (partners) {
			for (var index = 0, length = partners.length; index < length; index++) {
				var partner = partners[index];
				rows.push({
					packageName: partner.PartnerName,
					className: partner.PartnerName,
					value: partner.ReservationCount
				});
			}
			self.loadCharts(rows);
		});
	};
};
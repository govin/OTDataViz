var BubbleViewModel = function(options) {
	var self = this;

	this.displayBackButton = false;
	this.neighborhoodLabel = options.neighborhoodLabel;
	this.metroLabel = options.metroLabel;
	this.backLabel = options.backLabel;
	this.bubbleChartUrl = options.bubbleChartUrl;
	this.radialChartUrl = options.radialChartUrl;
	
	$.mockJSON.data.US_STATE = [
			'AK', 'AL', 'AR', 'AZ', 'CA', 'CO', 'CT', 'DC', 'DE', 'FL', 'GA', 'HI', 'IA', 'ID', 'IL', 'IN', 'KS',
			'KY', 'LA', 'MA', 'MD', 'ME', 'MI', 'MN', 'MO', 'MS', 'MT', 'NC', 'ND', 'NE', 'NH', 'NJ', 'NM', 'NV', 'NY',
			'OH', 'OK', 'OR', 'PA', 'RI', 'SC', 'SD', 'TN', 'TX', 'UT', 'VA', 'VT', 'WA', 'WI', 'WV', 'WY'
	];

	$.mockJSON(/foo\.json/, {
		"restaurants|50-50": [
			{
				"Rname": "@LOREM",
				"location": "@US_STATE",
				"reservationcount|50-100": 0,
				"id": "@LOREM"
			}
		]
	});

	this.loadRadials = function (id) {
		
		var url = self.radialChartUrl;
		if (typeof id !== "undefined" && id !== null) {
			url += "&metro=" + id;
		}
		
		//$.mockJSON(/boo\.json/, {
		//	"data|3-3": [
		//		{
		//			"CuisineName": "@LOREM",
		//			"CurrentReservationCount|650-999": 0
		//		}
		//	]
		//});

		//$.getJSON('boo/boo.json', function(json) {
		//	$("#firstRadial").data("kendoRadialGauge").value(json.data[0].CurrentReservationCount);
		//	$("#secondRadial").data("kendoRadialGauge").value(json.data[1].CurrentReservationCount);
		//	$("#thirdRadial").data("kendoRadialGauge").value(json.data[2].CurrentReservationCount);
		//	$("#firstRadialText").text(json.data[0].CuisineName);
		//	$("#secondRadialText").text(json.data[1].CuisineName);
		//	$("#thirdRadialText").text(json.data[2].CuisineName);
		//});
		
		$.getJSON(url, function (data) {
			$("#firstRadial").data("kendoRadialGauge").value(data[0].ReservationCount);
			$("#secondRadial").data("kendoRadialGauge").value(data[1].ReservationCount);
			$("#thirdRadial").data("kendoRadialGauge").value(data[2].ReservationCount);
			$("#firstRadialText").text(data[0].CuisineName);
			$("#secondRadialText").text(data[1].CuisineName);
			$("#thirdRadialText").text(data[2].CuisineName);
		});
	};

	this.loadCharts = function(restaurants) {
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
				.filter(function(d) {
					return !d.children;
				}))
			.enter().append("g")
			.attr("class", "node")
			.attr("transform", function(d) {
				return "translate(" + d.x + "," + d.y + ")";
			});

		node.append("title")
			.text(function(d) {
				return d.className + ": " + format(d.value);
			});

		var colorCol = [];

		node.append("circle")
			.attr("r", function(d) {
				return d.r;
			})
			.style("fill", function(d) {
				var assignedColor = color(d.packageName);

				var index = $.inArray(assignedColor, colorCol);

				if (index === -1) {
					colorCol.push(assignedColor);
					locationData.push({ location: d.packageName, color: assignedColor, count: d.value, id: d.id });
				} else {
					locationData[index].count += d.value;
				}
				return assignedColor;
			});

		node.append("text")
			.attr("dy", ".3em")
			.style("text-anchor", "middle")
			.text(function(d) {
				return d.className.substring(0, d.r / 3);
			});

		d3.select(self.frameElement).style("height", diameter + "px");

		$("#list").empty();

		locationData.sort(self.sortByCount);
		
		if (self.displayBackButton) {
			var backBtn = $('<div/>', { "class": "backButton" })
				.text(self.backLabel)
				.click(function() {
					self.displayBackButton = false;
					self.getBubbleData();
				});
			$("#list").append(backBtn);
			$("#area").text(self.neighborhoodLabel);
			$("#list").off("click", ".location");
		} else {
			$("#list").on("click", ".location", viewModel.fetchData);
			$("#area").text(self.metroLabel);
		}

		$.each(locationData, function(index, loc) {
			var ele = $('<div/>', { "style": "background-color:" + loc.color + ";", "class": "location", "id":"item"+index }).text(loc.location + " - " + loc.count);
			$(ele).data("id", loc.location);
			$("#list").append(ele);
		});
	};

	this.sortByCount = function(a, b) {
		return ((a.count > b.count) ? -1 : ((a.count < b.count) ? 1 : 0));
	};

	this.getBubbleData = function (id) {
		var url = self.bubbleChartUrl;
		var rows = [];
		if (typeof id !== "undefined" && id !== null) {
			url += "&metro=" + id;
		}
		
		$.getJSON(url, function (restaurants) {
			for (var index = 0, length = restaurants.length; index < length; index++) {
				var rest = restaurants[index];
				rows.push({ packageName: rest.Location, className: rest.RName, value: rest.ReservationCount });
			}
			self.loadCharts(rows);
		});
	};

	this.fetchData = function(event) {
		var id = $(event.target).data("id");
		self.getBubbleData(id);
		self.loadRadials(id);
		self.displayBackButton = true;
	};
};
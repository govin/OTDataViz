var BubbleViewModel = function() {
    var self = this;

    this.loadRadials = function () {
        $.mockJSON(/boo\.json/, {
            "data|3-3": [
                {
                    "CuisineName": "@LOREM",
                    "ReservationCount|650-999": 0
                }
            ]
        });

        //$.getJSON('boo/boo.json', function(json) {
    	//    $("#firstRadial").data("kendoRadialGauge").value(json.data[0].ReservationCount);
    	//    $("#secondRadial").data("kendoRadialGauge").value(json.data[1].ReservationCount);
    	//    $("#thirdRadial").data("kendoRadialGauge").value(json.data[2].ReservationCount);
        //    $("#firstRadialText").text(json.data[0].CuisineName);
        //    $("#secondRadialText").text(json.data[1].CuisineName);
        //    $("#thirdRadialText").text(json.data[2].CuisineName);
    	//});
	    
        $.getJSON('api/RadialChart', function (data) {
        	$("#firstRadial").data("kendoRadialGauge").value(parseInt(data[0].ReservationCount, 10));
        	$("#secondRadial").data("kendoRadialGauge").value(parseInt(data[1].ReservationCount, 10));
        	$("#thirdRadial").data("kendoRadialGauge").value(parseInt(data[2].ReservationCount, 10));
        	$("#firstRadialText").text(data[0].CuisineName);
        	$("#secondRadialText").text(data[1].CuisineName);
        	$("#thirdRadialText").text(data[2].CuisineName);
        });
    };

    this.loadCharts = function() {

        $.mockJSON.data.US_STATE = [
            'AK', 'AL', 'AR', 'AZ', 'CA', 'CO', 'CT', 'DC', 'DE', 'FL', 'GA', 'HI', 'IA', 'ID', 'IL', 'IN', 'KS',
            'KY', 'LA', 'MA', 'MD', 'ME', 'MI', 'MN', 'MO', 'MS', 'MT', 'NC', 'ND', 'NE', 'NH', 'NJ', 'NM', 'NV', 'NY',
            'OH', 'OK', 'OR', 'PA', 'RI', 'SC', 'SD', 'TN', 'TX', 'UT', 'VA', 'VT', 'WA', 'WI', 'WV', 'WY'
        ];

        $.mockJSON(/foo\.json/, {
            "restaurants|50-50": [
                {
                	"RName": "@LOREM",
                	"Location": "@US_STATE",
                	"ReservationCount|50-100": 0
                }
            ]
        });
	    

        //$.getJSON('foo/foo.json', function(json) {
        //    for (var index = 0, length = json.restaurants.length; index < length; index++) {
        //        var rest = json.restaurants[index];
        //        rows.push({ packageName: rest.location, className: rest.Rname, value: rest.reservationcount });
        //    }
    	//});
	    
    	//api/BubbleChartNA
        $.getJSON('api/BubbleChartNA', function (restaurants) {
	        //restaurants = restaurants.restaurants;
	        var rows = [];
    	    for (var index = 0, length = restaurants.length; index < length; index++) {
    	        var rest = restaurants[index];
    	        rows.push({ packageName: rest.Location, className: rest.RName, value: rest.ReservationCount });
    	    }
	        
    	    renderBubbleChart(rows);
	        renderRadials(rows);
        });
	    
        function renderRadials(json) {
			$("#firstRadial").data("kendoRadialGauge").value(json[0].CurrentReservationCount);
			$("#secondRadial").data("kendoRadialGauge").value(json[1].CurrentReservationCount);
			$("#thirdRadial").data("kendoRadialGauge").value(json[2].CurrentReservationCount);
			$("#firstRadialText").text(json[0].CuisineName);
			$("#secondRadialText").text(json[1].CuisineName);
			$("#thirdRadialText").text(json[2].CuisineName);
		}

        function renderBubbleChart(rows) {
        	$("#chart").fadeOut();
        	$("#chart").empty();
        	$("#chart").fadeIn();
	        
        	$("#list").empty();
	        
	        
        	var locationData = [];
        	var data = { children: rows };

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
						locationData.push({ location: d.packageName, color: assignedColor, count: d.value });
					} else {
						locationData[index].count += d.value;
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
	        
        	locationData.sort(self.sortByCount);
        	$.each(locationData, function (index, loc) {
        		var ele = $('<div/>', { "style": "background-color:" + loc.color + "; margin: 2px 0 2px 0; padding: 4px 0 4px 6px; border-radius: 4px;" }).text(loc.location + " - " + loc.count);
        		$("#list").append(ele);
        	});
        }
        
    };

    this.sortByCount = function(a, b) {
        return ((a.count > b.count) ? -1 : ((a.count < b.count) ? 1 : 0));
    };
};


var viewModel;
$(document).ready(function () {
    $("#loadCharts").on("click", function () {
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
    viewModel = new BubbleViewModel();
    viewModel.loadCharts();
    viewModel.loadRadials();
});
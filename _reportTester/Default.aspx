<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_reportTester.Default" %>

<%@ Register Assembly="Chart.js" Namespace="Chart.js" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
			<script>
				function hexToRgbA(hex, alpha) {
					if (!alpha) alpha = 1;
					var c;
					if (/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {
						c = hex.substring(1).split('');
						if (c.length == 3) {
							c = [c[0], c[0], c[1], c[1], c[2], c[2]];
						}
						c = '0x' + c.join('');
						return 'rgba(' + [(c >> 16) & 255, (c >> 8) & 255, c & 255].join(',') + ','+alpha+')';
					}
					throw new Error('Bad Hex');
				}

				function shadeRGBColor(color, percent) {
					if (percent == 0) return color;
					color = color.replace("rgb", "").replace("a", "").replace("(", "").replace(")", "");
					var colors = color.split(",");
					var alpha = ",1";
					while (colors.length > 3)
						alpha = "," + colors.pop();
					color = colors.join();
					var f = color.split(","), t = percent < 0 ? 0 : 255, p = percent < 0 ? percent * -1 : percent, R = parseInt(f[0]), G = parseInt(f[1]), B = parseInt(f[2]);
					return "rgba(" + (Math.round((t - R) * p) + R) + "," + (Math.round((t - G) * p) + G) + "," + (Math.round((t - B) * p) + B) + alpha +")";
				}

				Chart.cornerRadius = 20;
				Chart.dropShadow = true;
				Chart.shadowColor = "#000000";
				Chart.shadowBlur = 15;
				Chart.shadowOffsetX = 5;
				Chart.shadowOffsetY = 5;

				Chart.gradient = true;
				Chart.gradientStart =1.3;
				Chart.gradientEnd = .2;
				Chart.gradientStartShade = 0;
				Chart.gradientEndShade = .8;

				let lineEnabled =	function (dataset, options) {
					return Chart.helpers.valueOrDefault(dataset.showLine, options.showLines);
				}
				//let draw = Chart.controllers.line.prototype.draw;
				Chart.controllers.line.prototype.draw = function () {
					var me = this;
					var chart = me.chart;
					var meta = me.getMeta();
					var points = meta.data || [];
					var area = chart.chartArea;
					var ilen = points.length;
					var i = 0;

					Chart.helpers.canvas.clipArea(chart.ctx, area);
					if (lineEnabled(me.getDataset(), chart.options)) {
						chart.ctx.save();
						chart.ctx.shadowColor = Chart.shadowColor;
						chart.ctx.shadowBlur = Chart.shadowBlur;
						chart.ctx.shadowOffsetX = Chart.shadowOffsetX;
						chart.ctx.shadowOffsetY = Chart.shadowOffsetY;
						meta.dataset.draw();
						chart.ctx.restore();
					}

					Chart.helpers.canvas.unclipArea(chart.ctx);

					// Draw the points
					for (; i < ilen; ++i) {
						chart.ctx.save();
						chart.ctx.shadowColor = Chart.shadowColor;
						chart.ctx.shadowBlur = Chart.shadowBlur;
						chart.ctx.shadowOffsetX = Chart.shadowOffsetX;
						chart.ctx.shadowOffsetY = Chart.shadowOffsetY;
						points[i].draw(area);
						chart.ctx.restore();
						

					}

					//draw.apply(this, arguments);
										
				};
				
				let draw2 = Chart.controllers.doughnut.prototype.draw;
				Chart.controllers.pie.prototype.draw = function () {
					let ctx = this.chart.ctx;
					if (Chart.dropShadow) {
						ctx.shadowColor = Chart.shadowColor;
						ctx.shadowBlur = Chart.shadowBlur;
						ctx.shadowOffsetX = Chart.shadowOffsetX;
						ctx.shadowOffsetY = Chart.shadowOffsetY;
					}
					draw2.apply(this, arguments);
				}

				Chart.elements.Rectangle.prototype.draw = function () {
					var ctx = this._chart.ctx;
					var vm = this._view;
					var left, right, top, bottom, signX, signY, borderSkipped;
					var borderWidth = vm.borderWidth;
					if (Chart.dropShadow) {
						ctx.shadowColor = Chart.shadowColor;
						ctx.shadowBlur = Chart.shadowBlur;
						ctx.shadowOffsetX = Chart.shadowOffsetX;
						ctx.shadowOffsetY = Chart.shadowOffsetY;
					}
					// Set Radius Here
					// If radius is large enough to cause drawing errors a max radius is imposed
					var cornerRadius = Chart.cornerRadius;
					//console.log(this._chart);
					//console.log(this._chart.options.cornerRadius);

					if (!vm.horizontal) {
						// bar
						left = vm.x - vm.width / 2;
						right = vm.x + vm.width / 2;
						top = vm.y;
						bottom = vm.base;
						signX = 1;
						signY = bottom > top ? 1 : -1;
						borderSkipped = vm.borderSkipped || 'bottom';
					} else {
						// horizontal bar
						left = vm.base;
						right = vm.x;
						top = vm.y - vm.height / 2;
						bottom = vm.y + vm.height / 2;
						signX = right > left ? 1 : -1;
						signY = 1;
						borderSkipped = vm.borderSkipped || 'left';
					}

					// Canvas doesn't allow us to stroke inside the width so we can
					// adjust the sizes to fit if we're setting a stroke on the line
					if (borderWidth) {
						// borderWidth shold be less than bar width and bar height.
						var barSize = Math.min(Math.abs(left - right), Math.abs(top - bottom));
						borderWidth = borderWidth > barSize ? barSize : borderWidth;
						var halfStroke = borderWidth / 2;
						// Adjust borderWidth when bar top position is near vm.base(zero).
						var borderLeft = left + (borderSkipped !== 'left' ? halfStroke * signX : 0);
						var borderRight = right + (borderSkipped !== 'right' ? -halfStroke * signX : 0);
						var borderTop = top + (borderSkipped !== 'top' ? halfStroke * signY : 0);
						var borderBottom = bottom + (borderSkipped !== 'bottom' ? -halfStroke * signY : 0);
						// not become a vertical line?
						if (borderLeft !== borderRight) {
							top = borderTop;
							bottom = borderBottom;
						}
						// not become a horizontal line?
						if (borderTop !== borderBottom) {
							left = borderLeft;
							right = borderRight;
						}
					}

					ctx.beginPath();
					ctx.fillStyle = vm.backgroundColor;
					ctx.strokeStyle = vm.borderColor;
					ctx.lineWidth = borderWidth;

					// Corner points, from bottom-left to bottom-right clockwise
					// | 1 2 |
					// | 0 3 |
					var corners = [
						[left, bottom],
						[left, top],
						[right, top],
						[right, bottom]
					];
					width = corners[2][0] - corners[1][0];
					height = corners[0][1] - corners[1][1];
					x = corners[1][0];
					y = corners[1][1];
					// Find first (starting) corner with fallback to 'bottom'
					var borders = ['bottom', 'left', 'top', 'right'];
					var startCorner = borders.indexOf(borderSkipped, 0);
					if (startCorner === -1) {
						startCorner = 0;
					}

					function cornerAt(index) {
						return corners[(startCorner + index) % 4];
					}

					// Draw rectangle from 'startCorner'
					var corner = cornerAt(0);
					ctx.moveTo(corner[0], corner[1]);



						var radius = cornerRadius;
						
						// Fix radius being too large
						if (radius > Math.abs(height) / 2) {
							radius = Math.abs(height) / 2;
						} if (radius > Math.abs(width) / 2) {
							radius = Math.abs(width) / 2;
						}

						var xradius = radius;
						var yradius = radius;
						if (width < 0) xradius = xradius * -1;
						if (height < 0) yradius = yradius * -1;
						var cornerOrCurve = function (c1, c2, end1, end2, iscorner) {
							if (iscorner) {
								ctx.lineTo(c1, c2);
								ctx.lineTo(end1, end2);
							} else {
								ctx.quadraticCurveTo(c1, c2, end1, end2);
							}
						}

						ctx.moveTo(x + xradius, y);
						ctx.lineTo(x + width - xradius, y);
						ctx.quadraticCurveTo(x + width, y, x + width, y + yradius);
						ctx.lineTo(x + width, y + height - yradius);
						cornerOrCurve(x + width, y + height, x + width - xradius, y + height, !vm.horizontal)
						ctx.lineTo(x + xradius, y + height);
						cornerOrCurve(x, y + height, x, y + height - yradius, true) //lower left
						ctx.lineTo(x, y + yradius);
						cornerOrCurve(x, y, x + xradius, y, vm.horizontal)

					//}
						//ctx.save();
						if (Chart.gradient) {
							let gradientStart = 1 - Chart.gradientStart;
							let gradientEnd = 1 - Chart.gradientEnd;
							if (gradientStart == gradientEnd) gradientEnd+= 0.01
							var grd = ctx.createLinearGradient(x, y + (height * gradientStart), x, y + (height * gradientEnd));
							//if (height < 0) grd = ctx.createLinearGradient(0, height * 0.2, 0, height * 1.5 );
							var currentcolor = ctx.fillStyle;
							var rgbcolor = hexToRgbA(currentcolor);
							grd.addColorStop(0, shadeRGBColor(rgbcolor, Chart.gradientStartShade));
							grd.addColorStop(1, shadeRGBColor(rgbcolor, Chart.gradientEndShade));
							ctx.fillStyle = grd;
						}
						ctx.fill();
						ctx.fillStyle = currentcolor;
					if (borderWidth) {
						ctx.stroke();
					}
/*						ctx.save();
						ctx.clip();
						ctx.shadowColor = '#000';
						ctx.shadowBlur = 6;
						ctx.lineWidth = 2;
						ctx.stroke();
						ctx.restore();
						*/
						//ctx.lineWidth = 9;


				}; 
			</script>
			<%
			var dt = new System.Data.DataTable();
			dt.Columns.Add("val");
			dt.Columns.Add("Label");
			dt.Columns.Add("val2");

			dt.Rows.Add(new Object[] { 50, "January", 20});
			dt.Rows.Add(new Object[] { 44,"Febuary", 30 });
			dt.Rows.Add(new Object[] { 76, "March", 40 });
			dt.Rows.Add(new Object[] { 10, "April", 20});
			dt.Rows.Add(new Object[] { 66,"May", 30 });
			dt.Rows.Add(new Object[] { 44, "June", 40 });
			Chart1.data = dt;
				%>
			<cc1:Chart ID="Chart1" runat="server" Height="400px" Width="600px" chartType="bar" fillLineChart="True"></cc1:Chart>

        </div>
    </form>
</body>
</html>

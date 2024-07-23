var dataUrl;
var viewModel;
$(function () {
    viewModel = new ViewModel();
    console.log(dataUrl);
    viewModel.renderGrid(dataUrl);
    $("#grid").find("[data-role=pager]").css({ "margin-bottom": 0 });
});
var ViewModel = (function () {
    function ViewModel() {
    }
    ViewModel.prototype.renderGrid = function () {
        var grid = $("#grid").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
            grid.dataSource.read();
        }
        else {
            $("#grid").kendoGrid({
                dataSource: {
                    type: "json",
                    transport: {
                        read: function (options) {
                            $.ajax({
                                url: dataUrl,
                                dataType: "json",
                                type: "GET",
                                data: {},
                                success: function (result) {
                                    options.success(result);
                                }
                            });
                        },
                        cache: false
                    },
                    pageSize: 5,
                    schema: {
                        type: "json",
                        data: "Data",
                        total: function (response) {
                            return response.Data.length;
                        }
                    },
                    change: function () {
                        if (this.data().length <= 0) {
                            $("#message").show();
                            $("#grid").hide();
                            $("#pager").hide();
                        }
                        else {
                            $("#message").hide();
                            $("#grid").show();
                            $("#pager").show();
                            $("#warning").hide();
                        }
                    }
                },
                dataBound: function () {
                    jcf.destroyAll();
                },
                sortable: {
                    mode: "multiple",
                    allowUnsort: true,
                    showIndexes: true
                },
                scrollable: false,
                filterable: false,
                pageable: true,
                columns: [
                    {
                        field: "CompletedDate",
                        title: "Date",
                        attributes: { style: "text-align: center;" },
                        width: 200,
                        template: "#= kendo.toString(kendo.parseDate(CompletedDate, 'MM/dd/yyyy'), 'MM/dd/yyyy') #",
                        media: "(min-width: 450px)"
                    },
                    {
                        field: "Id",
                        title: "Id",
                        attributes: { style: "text-align: center;" },
                        media: "(min-width: 450px)"
                    },
                    {
                        field: "Title",
                        title: "Description",
                        attributes: { style: "text-align: center;" },
                        media: "(min-width: 450px)"
                    },
                    {
                        field: "Amount",
                        title: "Amount",
                        attributes: { style: "text-align: center;" },
                        format: "{0:c0}",
                        media: "(min-width: 450px)"
                    },
                    {
                        field: "",
                        title: "",
                        width: 200,
                        attributes: { style: "text-align: center; white-space: nowrap" },
                        template: kendo.template($("#grid-download-column-template").html()),
                        media: "(min-width: 450px)"
                    },
                    {
                        title: "Summary",
                        template: kendo.template($("#responsive-column-template").html()),
                        media: "(max-width: 450px)"
                    }
                ]
            });
        }
    };
    ViewModel.prototype.downloadFile = function (url) {
        history.pushState(null, "Orders", "/Order/Bills");
        window.location.replace(url);
    };
    return ViewModel;
}());
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJJbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFFQSxJQUFJLE9BQVksQ0FBQztBQUNqQixJQUFJLFNBQWMsQ0FBQztBQUVuQixDQUFDLENBQUM7SUFFQSxTQUFTLEdBQUcsSUFBSSxTQUFTLEVBQUUsQ0FBQztJQUM1QixPQUFPLENBQUMsR0FBRyxDQUFDLE9BQU8sQ0FBQyxDQUFDO0lBQ3JCLFNBQVMsQ0FBQyxVQUFVLENBQUMsT0FBTyxDQUFDLENBQUM7SUFPNUIsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxDQUFDLElBQUksQ0FBQyxtQkFBbUIsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxFQUFFLGVBQWUsRUFBRSxDQUFDLEVBQUUsQ0FBQyxDQUFDO0FBRXJFLENBQUMsQ0FBQyxDQUFDO0FBRUg7SUFBQTtJQTZHQSxDQUFDO0lBMUdDLDhCQUFVLEdBQVY7UUFFRSxJQUFNLElBQUksR0FBRyxDQUFDLENBQUMsT0FBTyxDQUFDLENBQUMsSUFBSSxDQUFDLFdBQVcsQ0FBQyxDQUFDO1FBQzFDLElBQUksSUFBSSxLQUFLLFNBQVMsSUFBSSxJQUFJLEtBQUssSUFBSSxFQUFFO1lBQ3ZDLElBQUksQ0FBQyxVQUFVLENBQUMsSUFBSSxFQUFFLENBQUM7U0FDeEI7YUFBTTtZQUNMLENBQUMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxTQUFTLENBQUM7Z0JBQ25CLFVBQVUsRUFBRTtvQkFDVixJQUFJLEVBQUUsTUFBTTtvQkFDWixTQUFTLEVBQUU7d0JBQ1QsSUFBSSxZQUFDLE9BQU87NEJBQ1YsQ0FBQyxDQUFDLElBQUksQ0FBQztnQ0FDTCxHQUFHLEVBQUUsT0FBTztnQ0FDWixRQUFRLEVBQUUsTUFBTTtnQ0FDaEIsSUFBSSxFQUFFLEtBQUs7Z0NBQ1gsSUFBSSxFQUFFLEVBQUU7Z0NBQ1IsT0FBTyxZQUFDLE1BQU07b0NBQ1osT0FBTyxDQUFDLE9BQU8sQ0FBQyxNQUFNLENBQUMsQ0FBQztnQ0FDMUIsQ0FB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RjthQUNGLENBQUMsQ0FBQztTQUNKO0lBQ0QsQ0FBQztJQUVILGdDQUFZLEdBQVosVUFBYSxHQUFHO1FBQ2QsT0FBTyxDQUFDLFNBQVMsQ0FBQyxJQUFJLEVBQUUsUUFBUSxFQUFFLGNBQWMsQ0FBQyxDQUFDO1FBQ2xELE1BQU0sQ0FBQyxRQUFRLENBQUMsT0FBTyxDQUFDLEdBQUcsQ0FBQyxDQUFDO0lBQy9CLENBQUM7SUFDSCxnQkFBQztBQUFELENBQUMsQUE3R0QsSUE2R0MiLCJzb3VyY2VzQ29udGVudCI6WyIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vU2NyaXB0cy90eXBpbmdzL2tlbmRvLXVpL2tlbmRvLXVpLmQudHNcIiAvPlxyXG5cclxudmFyIGRhdGFVcmw6IGFueTtcclxudmFyIHZpZXdNb2RlbDogYW55O1xyXG5cclxuJCgoKSA9PiB7XHJcblxyXG4gIHZpZXdNb2RlbCA9IG5ldyBWaWV3TW9kZWwoKTtcclxuICBjb25zb2xlLmxvZyhkYXRhVXJsKTtcclxuICB2aWV3TW9kZWwucmVuZGVyR3JpZChkYXRhVXJsKTtcclxuXHJcbiAgLy93aW5kb3cuc2V0SW50ZXJ2YWwoKCkgPT4ge1xyXG4gIC8vICAgIHJlbmRlck9yZGVyR3JpZChkYXRhVXJsKTtcclxuICAvLyAgfSxcclxuICAvLyAgNjAwMDApO1xyXG4gIC8vIHJlbW92ZXMgZXh0cmEgbWFyZ2luIGFwcGVhcmluZyBhdCBib3R0b20gb2YgZ3JpZFxyXG4gICAgJChcIiNncmlkXCIpLmZpbmQoXCJbZGF0YS1yb2xlPXBhZ2VyXVwiKS5jc3MoeyBcIm1hcmdpbi1ib3R0b21cIjogMCB9KTtcclxuIFxyXG59KTtcclxuXHJcbmNsYXNzIFZpZXdNb2RlbCB7XHJcbiAgdXJsOiBzdHJpbmc7XHJcblxyXG4gIHJlbmRlckdyaWQoKSB7XHJcblxyXG4gICAgY29uc3QgZ3JpZCA9ICQoXCIjZ3JpZFwiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgaWYgKGdyaWQgIT09IHVuZGVmaW5lZCAmJiBncmlkICE9PSBudWxsKSB7XHJcbiAgICAgIGdyaWQuZGF0YVNvdXJjZS5yZWFkKCk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICAkKFwiI2dyaWRcIikua2VuZG9HcmlkKHtcclxuICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICByZWFkKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgdXJsOiBkYXRhVXJsLFxyXG4gICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgIGRhdGE6IHt9LFxyXG4gICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIGNhY2hlOiBmYWxzZVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHBhZ2VTaXplOiA1LFxyXG4gICAgICAgICAgc2NoZW1hOiB7XHJcbiAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgdG90YWwocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBjaGFuZ2U6IGZ1bmN0aW9uKCkge1xyXG4gICAgICAgICAgICBpZiAodGhpcy5kYXRhKCkubGVuZ3RoIDw9IDApIHtcclxuICAgICAgICAgICAgICAkKFwiI21lc3NhZ2VcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICQoXCIjZ3JpZFwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgJChcIiNwYWdlclwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgJChcIiNtZXNzYWdlXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAkKFwiI2dyaWRcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICQoXCIjcGFnZXJcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICQoXCIjd2FybmluZ1wiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgZGF0YUJvdW5kOiAoKSA9PiB7XHJcbiAgICAgICAgICAvLyBuZWVkIHRvIHByZXZlbnQgamNmIHNlbGVjdCBwbHVnaW4gZnJvbSBhcHBseWluZyBpdHNlbGYgdG8gdGhlIEtlbmRvIGdyaWQncyBwYWdlciBlbGVtZW50XHJcbiAgICAgICAgICBqY2YuZGVzdHJveUFsbCgpO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc29ydGFibGU6IHtcclxuICAgICAgICAgIG1vZGU6IFwibXVsdGlwbGVcIixcclxuICAgICAgICAgIGFsbG93VW5zb3J0OiB0cnVlLFxyXG4gICAgICAgICAgc2hvd0luZGV4ZXM6IHRydWVcclxuICAgICAgICB9LFxyXG4gICAgICAgIHNjcm9sbGFibGU6IGZhbHNlLFxyXG4gICAgICAgIGZpbHRlcmFibGU6IGZhbHNlLFxyXG4gICAgICAgIHBhZ2VhYmxlOiB0cnVlLFxyXG4gICAgICAgIGNvbHVtbnM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiQ29tcGxldGVkRGF0ZVwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJEYXRlXCIsXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIHdpZHRoOiAyMDAsXHJcbiAgICAgICAgICAgIHRlbXBsYXRlOiBcIiM9IGtlbmRvLnRvU3RyaW5nKGtlbmRvLnBhcnNlRGF0ZShDb21wbGV0ZWREYXRlLCAnTU0vZGQveXl5eScpLCAnTU0vZGQveXl5eScpICNcIixcclxuICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiSWRcIixcclxuICAgICAgICAgICAgdGl0bGU6IFwiSWRcIixcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIlRpdGxlXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkRlc2NyaXB0aW9uXCIsXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJBbW91bnRcIixcclxuICAgICAgICAgICAgdGl0bGU6IFwiQW1vdW50XCIsXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpjMH1cIixcclxuICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIlwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJcIixcclxuICAgICAgICAgICAgd2lkdGg6IDIwMCxcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7IHdoaXRlLXNwYWNlOiBub3dyYXBcIiB9LFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNncmlkLWRvd25sb2FkLWNvbHVtbi10ZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgdGl0bGU6IFwiU3VtbWFyeVwiLFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNyZXNwb25zaXZlLWNvbHVtbi10ZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICBtZWRpYTogXCIobWF4LXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF1cclxuICAgICAgfSk7XHJcbiAgICB9XHJcbiAgICB9XHJcblxyXG4gIGRvd25sb2FkRmlsZSh1cmwpIHtcclxuICAgIGhpc3RvcnkucHVzaFN0YXRlKG51bGwsIFwiT3JkZXJzXCIsIFwiL09yZGVyL0JpbGxzXCIpO1xyXG4gICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UodXJsKTtcclxuICB9XHJcbn0iXX0=
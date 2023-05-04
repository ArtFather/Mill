function LoadData() {
    var recordData = [];
    $.ajax({
        type: "POST",
        url: "/Line/GetDataLine1/",
        async: false,
        success: function (data) {
            $.each(data, function (key, value) {
                recordData.push([value.id, value.order, value.part, value.qty, value.name, value.date, value.run, value.material, value.reason, value.routingCode, value.comment, value.wc, value.optimized, value.optimizedDate, value.rush, value.cutAt])
            })

            console.log(data);
        },
        failure: function (err) { }
    });

    $('#tbldata').DataTable({
        data: recordData,
        "columnDefs": [
            { "width": "80%", "targets": 0 }
        ]
    });
}
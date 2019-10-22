
$(document).ready(function () {
    $("#viewall").hide();
    var x = '@ViewBag.viewall';
    if (x == "no") {

    }
    else {
        $("#viewall").show();
    }
})

$(document).ready(function () {
    var x = document.getElementById("departmentlabel").innerText;
    var y = document.getElementById("genderlabel").innerText;
    if (x == "") {
        $("#cleardepartment").hide();
    }
    if (y == "") {
        $("#cleargender").hide();
    }
})


function genderfunc() {
    var x = document.getElementById("Genderselect").value;
    document.getElementById("genderlabel").innerText = x;
    document.getElementById("department").value = document.getElementById("departmentlabel").innerText;
    document.getElementById("gender").value = document.getElementById("genderlabel").innerText;
    $("#filterform").submit();
}


function departmentfunc() {
    var x = document.getElementById("Departmentselect").value;
    document.getElementById("departmentlabel").innerText = x;
    document.getElementById("department").value = document.getElementById("departmentlabel").innerText;
    document.getElementById("gender").value = document.getElementById("genderlabel").innerText;
    $("#filterform").submit();
}

function selectAll(source) {
    checkboxes = document.getElementsByName('boxes[]');

    for (var i in checkboxes) {
        checkboxes[i].checked = source.checked;

    }

}

$(document).ready(function () {
    $("#delete").click(function () {
        var selectedIDs = new Array();
        $('input:checkbox.checkBox').each(function () {
            if ($(this).prop('checked')) {
                selectedIDs.push($(this).val());
            }
        });

        var options = {};
        options.url = "/Admin/Delete";
        options.type = "POST";
        options.data = JSON.stringify(selectedIDs);
        options.contentType = "application/json";
        options.dataType = "json";
        options.success = function (msg) {
            alert(msg);
        };

        $.ajax(options);

    });
});

$(document).ready(function () {
    $("#myInput").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#myTable tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});

$(document).ready(function () {
    $(".fa1").hide();

    var x = '@TempData["sorting"]';
    if (x == "") {
        $("#eidasc").show();
    }
    if (x == "eid") {
        $("#eidasc").show();
    }
    if (x == "eid_desc") {
        $("#eiddesc").show();
    }
    if (x == "name") {
        $("#nameasc").show();
    }
    if (x == "name_desc") {
        $("#namedesc").show();
    }
    if (x == "Date") {
        $("#dobasc").show();
    }
    if (x == "date_desc") {
        $("#dobdesc").show();
    }
    if (x == "gender") {
        $("#genderasc").show();
    }
    if (x == "gender_desc") {
        $("#genderdesc").show();
    }
    if (x == "JoinDate") {
        $("#joiningasc").show();
    }
    if (x == "join_data_desc") {
        $("#joiningdesc").show();
    }
    if (x == "dept") {
        $("#deptasc").show();
    }
    if (x == "dept_desc") {
        $("#deptdesc").show();
    }
    if (x == "email") {
        $("#emailasc").show();
    }
    if (x == "email_desc") {
        $("#emaildesc").show();
    }
    if (x == "contact") {
        $("#contactasc").show();
    }
    if (x == "contact_desc") {
        $("#contactdesc").show();
    }
})

function PagerClick(pageNumber) {
    var sortorder = '@TempData["sorting"]';
    var genderSelectedValue = '@TempData["genderfilter"]';

    var searchstring = '@TempData["searchstring"]';


    var DepartmentSelectedValue = '@TempData["departmentfilter"]';
    if (genderSelectedValue == "undefined")
        genderSelectedValue = "";
    if (DepartmentSelectedValue == "undefined")
        DepartmentSelectedValue = "";
    var string_url = "http://localhost:61130/Admin/Index/?page=" + pageNumber + "&searchString=" + searchstring + "&sortOrder=" + sortorder + "&departmentfilter=" + DepartmentSelectedValue + "&genderfilter=" + genderSelectedValue;
    window.location = string_url;
}
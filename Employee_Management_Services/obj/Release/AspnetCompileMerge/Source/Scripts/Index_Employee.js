
$(document).ready(function () {
    $(".viewbtn").hide();
    var x = '@ViewBag.viewall';
    if (x == "no") {

    }
    else {
        $(".viewbtn").show();
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
});


function PagerClick(pageNumber) {
    var sortorder = '@TempData["sort"]';
    var genderSelectedValue = '@TempData["genderfilter"]';
    var searchstring = '@TempData["searchstring"]';
    var DepartmentSelectedValue = '@TempData["departmentfilter"]';
    //var genderSelectedValue = $("#Genderselect").val();
    if (genderSelectedValue == "undefined")
        genderSelectedValue = "";
    //var DepartmentSelectedValue = $("#Departmentselect").val();
    if (DepartmentSelectedValue == "undefined")
        DepartmentSelectedValue = "";
    var string_url = "http://localhost:61130/Employee/Index?id=" + @Session["Security"] + "&searchString=" + searchstring + "&page=" + pageNumber + "&sortOrder=" + sortorder + "&departmentfilter=" + DepartmentSelectedValue + "&genderfilter=" + genderSelectedValue ;
    window.location = string_url;
}


function genderfunc()
{
    var x = document.getElementById("Genderselect").value;
    document.getElementById("genderlabel").innerText = x;
    document.getElementById("department").value = document.getElementById("departmentlabel").innerText;
    document.getElementById("gender").value = document.getElementById("genderlabel").innerText;
    $("#filterform").submit();
}


function departmentfunc()
{
    var x = document.getElementById("Departmentselect").value;
    document.getElementById("departmentlabel").innerText = x;
    document.getElementById("department").value = document.getElementById("departmentlabel").innerText;
    document.getElementById("gender").value = document.getElementById("genderlabel").innerText;
    $("#filterform").submit();
}

     



$(document).ready(function () {
    $(".fa1").hide();

    var x = '@TempData["sort"]';
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
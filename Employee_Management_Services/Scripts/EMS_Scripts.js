$(function () {
    $(".datefield").datepicker({ dateFormat: 'dd-M-yy', changeYear: true });
    $(".datefield").addClass("form-control");
});


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












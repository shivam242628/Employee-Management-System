﻿$(function () {
    $(".datefield").datepicker({ dateFormat: 'dd-M-yy', changeYear: true });
    $(".datefield").addClass("form-control");
});

document.getElementByClass("join").readOnly = true;
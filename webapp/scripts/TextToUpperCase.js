$("input[type='text'], textarea").on("input", function () {
    var teks = $(this).val();
    $(this).val(teks.toUpperCase());
})
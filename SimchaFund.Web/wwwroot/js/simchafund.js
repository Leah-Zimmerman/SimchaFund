$(() => {
    $("#addsimcha").on('click', function () {
        new bootstrap.Modal($("#simcha-modal")[0]).show()
    })
    $("#deposittable").on('click', '.btn-success', function () {
        const button = $(this);
        const id = button.val();
        console.log(id);
        const name = $(this).val('name');
        new bootstrap.Modal($("#deposit-modal")[0]).show()
        $(".modal-footer .btn-primary").val(id);
        console.log($(".modal-footer .btn-primary").val());
    })
})
$(() => {
    let name = "";
    let id = 0;
    let checked = false;
    let counter = 0;
    $("#addsimcha").on('click', function () {
        new bootstrap.Modal($("#simcha-modal")[0]).show()
    })
    $("#addcontributor").on('click', function () {
        new bootstrap.Modal($("#addcontributor-modal")[0]).show()
    })
    $("tbody").on('click', '#editcontributor', function () {
        contributorId = $(this).data('id');
        console.log(contributorId);
        firstname = $(this).data('firstname');
        lastname = $(this).data('lastname');
        cell = $(this).data('cell');
        checked = $(this).data('alwaysinclude');
        $('.modal #includedcheckbox').prop('checked', false);
        new bootstrap.Modal($("#editcontributor-modal")[0]).show()
        $('.modal #contributorId').val(contributorId);
        $('.modal #firstname').val(firstname);
        $('.modal #lastname').val(lastname);
        $('.modal #cellnumber').val(cell);
        console.log(checked);
        if (checked == "True") {
            $('.modal #includedcheckbox').prop('checked', true);
        }
    })
    $("tbody").on('click', '.btn-success', function () {
        name = $(this).data('name');
        id = $(this).data('id');
        new bootstrap.Modal($("#deposit-modal")[0]).show();
        $("#modaltitle").text("Deposit for " + name);
        $(".modal-footer .btn-primary").val(id);
    })
    $("tbody").on('click', '#showhistory', function () {
        contributorId = $(this).data('contributor-id');
        $(this).val(contributorId);
    })

    $("tbody").on('click', '#contributions', function () {
        simchaId = $(this).data('simcha-id');
        $(this).val(simchaId);
        console.log(simchaId);
    })
    $("#update").on('click', function () {
        $(".d-table-row").each(function () {
            const row = $(this);
            const inputs = row.find('input');
            inputs.each(function () {
                const input = $(this);
                if (input.is(':checked')) {
                    console.log('checked');
                    $(this).val("true");
                    console.log(input.val());
                }
                const name = input.attr('name');
                var IndexOfDot = name.indexOf('.');
                const attrName = name.substring(IndexOfDot + 1);
                input.attr('name', `contributorRows[${counter}].${attrName}`);
            });
            counter++;
        });
    })
})
function Search() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable");
    tr = table.getElementsByTagName("tr");
    console.log(tr.length)

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[1];
        console.log('further in')
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}
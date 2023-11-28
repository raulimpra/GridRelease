// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Form validation when class="needs-validation" is used
(function () {
    'use strict'

    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.querySelectorAll('.needs-validation')

    // Loop over them and prevent submission
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault()
                    event.stopPropagation()
                }

                form.classList.add('was-validated')
            }, false)
        })
})();

// Form validation when class="dates-validation" is used
(function () {
    'use strict'

    var forms = document.querySelectorAll('.dates-validation')

    if (forms.length > 0) {
        jQuery.validator.addMethod("greaterThan",
            function (value, element, params) {
                return new Date(value) >= new Date($(params).val());
            }, "Final menor a Inicio.");
    }

    // Loop over them and prevent submission
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            $(form).validate({
                rules: {
                    end: { greaterThan: "#start" }
                },
                errorClass: "invalid-feedback",
                errorElement: "div",
                highlight: function (element) {
                    $(element).addClass("is-invalid").removeClass("is-valid");
                },
                unhighlight: function (element) {
                    $(element).addClass("is-valid").removeClass("is-invalid");
                }
            });
        })
})();

// Add title to column truncated by overflow:hidden
$('.upload-error tr td:nth-child(5)').bind('mouseenter', function(){
    var $this = $(this);

    if(this.offsetWidth < this.scrollWidth && !$this.attr('title')){
        $this.attr('title', $this.text());
    }
});

function ShowSuccessMessage(msg) {
    var main = $(".main-message");
    var template = $(".main-message .d-none.alert-success");

    var element = $(template[0]).clone().removeClass("d-none");
    element.find("span").text(msg);

    main.append(element);
}
function ShowErrorMessage(msg, detail) {
    var main = $(".main-message");
    var template = $(".main-message .d-none.alert-danger");

    var element = $(template[0]).clone().removeClass("d-none");
    element.find("input").val(detail);
    element.find("span").text(msg);

    main.append(element);
}
function ClearAllMessages() {
    $(".main-message .alert").not(".d-none").remove();
}
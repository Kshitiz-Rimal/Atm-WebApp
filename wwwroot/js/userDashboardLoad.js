$(document).ready(function () {

    const $changePinButton = $("#changePinButton");

    if ($changePinButton.length) {
        // Add a click event listener to log when the button is clicked
        console.log('Button found');

        // Trigger the click event on the button
        $changePinButton.click();
    }
    // Custom validation rule for numbers divisible by 500
    $.validator.addMethod("divisibleBy500", function (value, element) {
        return this.optional(element) || (value % 500 === 0);
    }, "Please enter a number divisible by 500.");

    // Validate the form
    $("#modalForm").validate({
        rules: {
            inputField: {
                required: true,
                number: true,
                divisibleBy500: true
            }
        },
        messages: {
            inputField: {
                required: "This field is required.",
                number: "Please enter a valid number.",
                divisibleBy500: "Please enter a number divisible by 500."
            }
        },
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "div",
        errorPlacement: function (error, element) {
            error.addClass("invalid-feedback");
            error.insertAfter(element);
        },
        highlight: function (element, errorClass, validClass) {
            $(element).addClass(errorClass).removeClass(validClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass(errorClass).addClass(validClass);
        },

        submitHandler: function (form, event) {
            event.preventDefault(); // Prevent default form submission


            // Get the value of the input field
            var value1 = $('#inputField').val();
            var value2 = $('#accountNumber').val();
            console.log(value1);
            console.log(value2);

            // Send the data via AJAX
            $.ajax({
                url: '/Dashboard/WithdrawMoney',
                type: 'POST',
                data: {
                    withdrawAmount: value1,
                    accountNumber: value2
                },
                success: function (response) {
                    // Check if the response indicates success
                    if (response.success) {
                        // Show success message
                        var modal = bootstrap.Modal.getInstance(document.getElementById('withdrawModal'));
                        modal.hide();
                        $('#successModalLabel').text("Withdraw Successful")
                        $('#successModalBody').text(response.message);
                        $('#successModal').modal('show');

                        setTimeout(function () {
                            window.location.href = '/Dashboard/UserDashboard';
                        }, 3000);

                    } else {
                        var modal = bootstrap.Modal.getInstance(document.getElementById('withdrawModal'));
                        modal.hide();
                        $('#successModalLabel').text("Withdraw Failed")
                        $('#successModalBody').text(response.message);
                        $('#successModal').modal('show');

                        setTimeout(function () {
                            $('#successModal').modal('hide');
                        }, 3000);
                        // Handle failure case
                        $('#successMessage').removeClass('alert-success').addClass('alert-danger').text('Withdrawal failed: ' + response.message).show();
                    }
                },
                error: function (error) {
                    // Handle the error response
                    console.log("Error:", error);
                    alert('An error occurred. Please try again.');
                }
            });
        }
    });
    document.getElementById('amountButton').addEventListener('click', function () {
        var modal = bootstrap.Modal.getInstance(document.getElementById('withdrawModal'));
        modal.hide();
    });

    document.getElementById('customAmountButton').addEventListener('click', function () {
        var modal = bootstrap.Modal.getInstance(document.getElementById('amountModal'));
        modal.hide();
    });

    //window.onload = function () {
    //    if (document.getElementById("changePinButton")) {

    //        document.getElementById("changePinButton").click();
    //    }
    //};

    //document.addEventListener('DOMContentLoaded', function () {
    //    // Select the button element
    //    var button = document.getElementById('changePinButton');

    //    // Simulate a button click
    //    button.click();
    //});

    document.addEventListener("DOMContentLoaded", function () {
        var buttons = document.querySelectorAll('.amount-button');
        buttons.forEach(function (button) {
            button.addEventListener('click', function () {
                var value1 = this.getAttribute('data-value1');
                var value2 = this.getAttribute('data-value2');

                // Send the value to the server using AJAX
                $.ajax({
                    url: '/Dashboard/WithdrawMoney', // Check this URL
                    type: 'POST',
                    data: {
                        withdrawAmount: value1,
                        accountNumber: value2,
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (response) {
                        // Check if the response indicates success
                        if (response.success) {
                            // Show success message
                            var modal = bootstrap.Modal.getInstance(document.getElementById('withdrawModal'));
                            modal.hide();
                            $('#successModalLabel').text("Withdraw Successful")
                            $('#successModalBody').text(response.message);
                            $('#successModal').modal('show');

                            setTimeout(function () {
                                window.location.href = '/Dashboard/UserDashboard';
                            }, 3000);

                        } else {
                            var modal = bootstrap.Modal.getInstance(document.getElementById('withdrawModal'));
                            modal.hide();
                            $('#successModalLabel').text("Withdraw Failed")
                            $('#successModalBody').text(response.message);
                            $('#successModal').modal('show');

                            setTimeout(function () {
                                $('#successModal').modal('hide');
                            }, 3000);
                            // Handle failure case
                            $('#successMessage').removeClass('alert-success').addClass('alert-danger').text('Withdrawal failed: ' + response.message).show();
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                        $('#successMessage').removeClass('alert-success').addClass('alert-danger').text('An error occurred while processing your request.').show();
                    }
                });
            });
        });
    });
});
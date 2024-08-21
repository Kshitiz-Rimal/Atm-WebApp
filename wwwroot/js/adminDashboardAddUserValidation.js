

$(document).ready(function () {
    // Custom validation method for Customer Name
    $.validator.addMethod("validCustomerName", function (value, element) {
        return /^[A-Z][a-zA-Z]* [A-Z][a-zA-Z]*$/.test(value); // Match first and last name with capital initials
    }, "Customer name must start with a capital letter for both first and last name.");

    // Custom validation method for Account Number
    $.validator.addMethod("validAccountNumber", function (value, element) {
        return /^\d{10}$/.test(value); // Match exactly 10 digits
    }, "Account number must be exactly 10 digits.");

    // Custom validation method for Balance
    $.validator.addMethod("validBalance", function (value, element) {
        return /^[0-9]*$/.test(value); // Match only digits
    }, "Balance must contain only digits.");

    $("#addUserForm").validate({
        rules: {
            CustomerName: {
                required: true,
                validCustomerName: true
            },
            AccountNumber: {
                required: true,
                validAccountNumber: true
            },
            Balance: {
                required: true,
                validBalance: true
            }
        },
        messages: {
            CustomerName: {
                required: "Customer name is required."
            },
            AccountNumber: {
                required: "Account number is required."
            },
            Balance: {
                required: "Balance is required."
            }
        },
        errorPlacement: function (error, element) {
            // Add the 'error' class to the error message container
            error.addClass('error');
            // Place the error message in a specific container
            error.insertAfter(element); // Example: insert after the input field
        },
        submitHandler: function (form, event) {

            event.preventDefault(); // Prevent default form submission
            var formData = {
                CustomerName: $('#customerName').val(),
                AccountNumber: $('#accountNumber').val(),
                Balance: $('#balance').val()
            };
            // AJAX form submission logic here
            $.ajax({
                type: "Post",
                url: '/Dashboard/AddUser',
                contentType: "application/json",
                data: JSON.stringify(formData),
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        window.location.reload();
                        alert("Customer Added Successfully");
                    } else {
                        var modal = bootstrap.Modal.getInstance(document.getElementById('addUserModal'));

                        // Update the modal content with the returned partial view
                        $('#errorContainer').text(response.message).removeClass('d-none');
                        setTimeout(function () {
                            
                            $('#errorContainer').addClass('d-none').text('');
                            modal.hide();
                        }, 2000);
                    }
                },
                error: function (xhr, status, error) {
                    console.error("An error occurred while submitting the form: ", error);
                }
            });

        }
    });
});
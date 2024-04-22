//country code
var phone1 = document.querySelector("#phone");
var phoneInput1 = window.intlTelInput(phone1, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    nationalMode: false,
    showSelectedDialCode: true,
    autoInsertDialCode: true,
    formatOnDisplay: false,
});
var phone2 = document.querySelector("#phone1");
var phoneInput2 = window.intlTelInput(phone2, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    nationalMode: false,
    showSelectedDialCode: true,
    autoInsertDialCode: true,
    formatOnDisplay: false,
});
//file upload
function displayFilename() {
    var input = document.getElementById('myFile');
    var output = document.getElementById('selectedFilename');
    output.textContent = input.files[0].name;
}
//sweet alert
swal({
    title: "Information",
    text: "When submitting a request, you must provide the correct contact information for the patient or the responsibly party. Failure to provide the correct email and phone number will delay service or be declined.",
    type: "warning",
    confirmButtonColor: "#0dcaf0",
});


   






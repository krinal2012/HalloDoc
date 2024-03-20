
//country code
const phoneInputField = document.querySelector("#phone");
const phoneInput = window.intlTelInput(phoneInputField, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});
const phoneInputField1 = document.querySelector("#phone1");
const phoneInput1 = window.intlTelInput(phoneInputField1, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});
//file upload
function displayFilename() {
    var input = document.getElementById('myFile');
    var output = document.getElementById('selectedFilename');
    output.textContent = input.files[0].name;
}
//function displayFilename() {
//    var input = document.getElementById('myFile');
//    var output = document.getElementById('selectedFilename');
//    for (let i = 0; i < selectedFilename.length; i++) {
//        output.append = input.files[i].name;
//    }   
//}

//sweet alert
swal({
    title: "Information",
    text: "When submitting a request, you must provide the correct contact information for the patient or the responsibly party. Failure to provide the correct email and phone number will delay service or be declined.",
    type: "warning",
    confirmButtonColor: "#0dcaf0",
});






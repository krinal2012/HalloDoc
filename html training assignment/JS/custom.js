

// $("#files").change(function (event) {
//     event.preventDefault(); 
//     console.log("function");
//     filename = this.files[0].name;
//     $("#choosenfile").text(filename);
// });
function displayFilename() {
    var input = document.getElementById('myFile');
    var output = document.getElementById('selectedFilename');
    output.textContent = input.files[0].name;
}

// side bar
function sidebar(event) {
   
    var temp = document.getElementById('sidebar');
    var dis = temp.style.display;
    if (dis === 'block') {
        temp.style.display = 'none';
        document.getElementById('main').style.width = "100vw";
    }
    else {
        temp.style.display = 'block';
        document.getElementById('main').style.width = "85vw";
    }
}

// tabs
function openTab(evt, tabName) {
    evt.preventDefault();
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.className += " active";
}


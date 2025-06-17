var myModal = new bootstrap.Modal(document.getElementById('myModal'));
var lastFolder = "";
var lastFiles = [];
var baseurl = "home";
var icon_image='🖼️';
var icon_video = '🎞️';
var icon_doc='📄';
var icon_zip='🗜';
var icon_audio='🔊';
var icon_unknown='⛘';


function showFile2(path, title, extension) {

    extension = fileIcon(extension.toLowerCase());
    debugger


    $("#showModal").html(`<div class="mb-2 pb-1 px-2 text-left border-bottom">
                            <button onclick="prevFile('${path}')" class="btn btn-warning mr-1" style="margin-right:2px" ><</label>
                            <button onclick="nextFile('${path}')" class="btn btn-warning " >></label>
                            <button onclick="pushSelectedfilesToParent('${path}')" class="btn btn-warning float-right" style="float:right" >✔</button>
                        </div>`);

    if (extension == icon_audio) {
        $("#showModal").append(`<audio controls class="w-100">
                                          <source src="${path}" type="audio/ogg">
                                          <source src="${path}" type="audio/mpeg">
                                          Your browser does not support the audio element.
                                      </audio>`);
    }   
    else if (extension == icon_image) {
        $("#showModal").append(`<img src="${path}" style="max-width: 100%;" />`);
    }
    else {
        $("#showModal").append(`<span>${path}</span><br />
                                <a href="${path}" target="_blank">download</a>`);
    }

    $(".modal-title").html(title);
    myModal.show();
}

function addDirectoryModal(path, title, dir) {

    $("#showModal").html(`
               <input type="hidden" class="form-control" id="parent_dir" value="${dir}" >
               <input type="hidden" class="form-control" id="parent_path" value="${path}" >
               <div class="mb-3">
                 <label for="exampleInputEmail1" class="form-label">Name</label>
                 <input type="text" class="form-control" id="dir_name" >
               </div>
               <button type="button" onclick="addSubDirectory()" class="btn btn-primary">Submit</button>
            `)
    $(".modal-title").html(title);
    myModal.show();
}

function addSubDirectory() {
    $.post(`/${baseurl}/CreateDir`, { d: $("#parent_path").val(), name: $("#dir_name").val() })
        .done(function () {
            myModal.hide();
            
            lastFolder = "";
            loadSub($("#parent_dir").val(), $("#parent_path").val())
            $("#showModal").html(``);
        })
        .fail(function () {
            alert("error");
        });
}

function removeDir(path, parentD, divId) {

    $.post(`/${baseurl}/RemoveDir`, { d: path })
        .done(function () {


            loadSub(divId, parentD)

        })
        .fail(function (er) {
            alert(er["System.IO.IOException"]);
        });
}

function loadSub(i0, d) {
    debugger
    $(`.folder`).removeClass('open');
    if (lastFolder == d && d != '') {
        $(`#dir_${i0}`).html('');
        $("#files_").html('');
        $(`.d_${i0}`).removeClass('open');
        lastFolder = "";
        return;
    }
    $(`.d_${i0}`).addClass('open');
    lastFolder = d;
    $("#fullPathOfDir").val(d);
    $("#files_").html('<div class="spinner-border" role="status"></div>');
    $.get(`/${baseurl}/SubDir?d=` + d, function (data, status) {

        lastFiles = data.files;
        let html = '';
        for (let i = 0; i < data.dirs.length; i++) {
            let j = i;
            html += `<div class="">
                                <div class=""  style="display:inline-flex">
                                    <label class="p-2 folder d_${i0}_${j}" onclick="loadSub('${i0}_${j}','${data.dirs[j].path}')">${data.dirs[j].text}</label>
                                    <div class="dropdown">
                                        <button class="btn btn-link btn-sm" type="button"  data-bs-toggle="dropdown" >
                                            ⋮
                                        </button>
                                        <ul class="dropdown-menu" >
                                            <li><a class="dropdown-item" onclick="uploadFile('${data.dirs[j].path}','${d}','${i0}_${j}')" href="#">📤Upload File</a></li>
                                            <li><a class="dropdown-item" onclick="addDirectoryModal('${data.dirs[j].path}','${data.dirs[j].text}','${i0}_${j}')" href="#">➕Sub Directory</a></li>
                                            <li><hr class="dropdown-divider"></li>
                                            <li><a class="dropdown-item" onclick="removeDir('${data.dirs[j].path}','${d}','${i0}')" href="#">🗑️Remove</a></li>
                                        </ul>
                                    </div>
                                </div>
                                <div id="dir_${i0}_${j}" class="ml-3 ms-3" style="" > </div>
                            </div>`;
        }
        if (d == '' || d == '/')
            $(".mainDirecories").html(html);
        else
            $("#dir_" + i0).html(html);


        html = '';
        for (let i = 0; i < data.files.length; i++) {
            let j = i;
            let f = data.files[j];
            html += `<div class="">
                               <div class="py-1 px-2" style="display:inline-flex">
                                   <label style='white-space: nowrap;'>
                                      <input type="checkbox" value='${f.path}' class="_files" />
                                      ${fileIcon(f.extension)}
                                   </label>
                                   <label onclick="showFile2('${f.path}','${f.text}','${f.extension}')" >
                                        ${f.text}
                                   </label>
                                   <small style="background: yellow;">(${f.length})</small>
                              </div>
                          </div>`;
        }
        if (data.files.length == 0)
            $("#files_").html(`<div class="ml-4 pb-2">--folder is empty--</div>`);
        else
            $("#files_").html(html)
    });
}

function fileIcon(extension) {
    if (extension.match(/\.(jpg|jpeg|png|gif|webp|bmp)$/i)) return icon_image;
    if (extension.match(/\.(mp4|m4a|avi|mov|f4v)$/i)) return icon_video;
    if (extension.match(/\.(txt|docx|doc)$/i)) return icon_doc;
    if (extension.match(/\.(rar|zip|7zip)$/i)) return icon_zip;
    if (extension.match(/\.(mp3)$/i)) return icon_audio;
    return icon_unknown;
}

function pushSelectedfilesToParent(filePath) {
    window.parent.postMessage(filePath, '*');
    myModal.hide();
}

function nextFile(filePath) {

    let ind = lastFiles.findIndex(x => x.path == filePath);
    ind++;
    if (ind == lastFiles.length)
        return;
    let f = lastFiles[ind];
    showFile2(`${f.path}`, `${f.text}`, `${f.extension}`);
}

function prevFile(filePath) {

    let ind = lastFiles.findIndex(x => x.path == filePath);
    if (ind == 0)
        return;
    ind--;
    let f = lastFiles[ind];
    showFile2(`${f.path}`, `${f.text}`, `${f.extension}`);
}

function getSelectedFilesandPushToParent() {
    var checkedValue = '';
    var inputElements = document.getElementsByClassName('_files');

    for (var i = 0; inputElements[i]; ++i)
        if (inputElements[i].checked)
            checkedValue += inputElements[i].value + '**#**';

    pushSelectedfilesToParent(checkedValue);
}

function togglecheckAllByClass() {
    const checkboxes = document.querySelectorAll(`._files`);
    checkboxes.forEach(checkbox => {
        checkbox.checked = document.getElementById('check_all_file').checked;
    });
}

var _lastuploadFilePath;
function uploadFile(path, d, i0) {
    _lastuploadFilePath = { path, d, i0 };
    document.getElementById('lastuploadFileInput').click();
}

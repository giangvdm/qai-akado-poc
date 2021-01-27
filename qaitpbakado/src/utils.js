let actionUrl = "http://localhost:33450/";

function getCurrentFormContext() {
    return localStorage.getItem("formContext");
}

function getCurrentCif() {
    return localStorage.getItem("cif") ? localStorage.getItem("cif") : null;
}

function getCurrentBranch() {
    return localStorage.getItem("branch");
}

function getCurrentUser() {
    return localStorage.getItem("user");
}

function post(reqObj) {
    if (typeof reqObj === "object" && reqObj !== null) {
        let jsonStr = JSON.stringify(reqObj);
        console.log(jsonStr);

        let ajaxReq = $.ajax({
            url: actionUrl,
            method: "POST",
            contentType: "application/json",
            data: jsonStr,
            error: function(e) {
                console.log("Error while posting data to server!");
                console.log(e);
            }
        });
    }
}

function sendPost(formName, processName, actor, step, isLastStep = false) {
    // init request object
    let formContext = getCurrentFormContext(),
        currentCif = getCurrentCif(),
        currentBranch = getCurrentBranch(),
        currentUser = getCurrentUser();
    let obj = new MyObj(
        formContext,
        currentCif,
        currentBranch,
        formName,
        processName,
        actor,
        currentUser,
        step,
        isLastStep
    );
    // send post request
    post(obj);
}
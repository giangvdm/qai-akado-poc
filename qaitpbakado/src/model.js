class MyObj {
    constructor(id, cif, branch, formName, processName, actor, user, step, isLastStep) {
        this.id = id,
        this.cif = cif;
        this.url = window.location.href;
        this.branch = branch;
        this.form = formName;
        this.process = processName;
        this.step = step;
        this.isLastStep = isLastStep;
        this.actor = actor;
        this.user = user;
        this.startTime = new Date().getTime();
        this.browser = navigator.userAgent;
    }
}

/**
 * ////////////////////////////////
 * // Mo CIF va TK nhanh STDCIF1 //
 * ////////////////////////////////
 */

/**
 * Initialize
 */
// event watching for branch switching and user logging in
// event watching for and switching form context and cif changes
$(document).on('mouseenter', 'iframe[id^="ifr_LaunchWin"]', function(e) {
    let cifInput = $(e.currentTarget.contentDocument).find('input[name="CUSTNO"]');
    let cifVal = $(cifInput).val(),
        currentBranch = $('a#Branch_Menu').text(),
        currentUser = $('a#LoggedUser_Menu').text();

    localStorage.setItem("formContext", e.currentTarget.name);
    localStorage.setItem("cif", cifVal);
    localStorage.setItem("branch", currentBranch);
    localStorage.setItem("user", currentUser);
});

/**
 * Step 1: Maker
 */
// Add new
$(document).on('click', 'li#New > a.TBitem', function() {
    sendPost('STDCIF1', 'Add new', 'Maker', 1); // start add new
});

// Save
$(document).on('mousedown', 'li#Save > a.TBitem', function() {
    sendPost('STDCIF1', 'Save', 'Maker', 2); // end add new, start check

    let currFrameBody = $(this).parents('body');
    let alertIfr = $(currFrameBody).find('#ifr_AlertWin');

    $(alertIfr).on('load', function() {
        let frameTitle = $(this.contentDocument).find('h1.WNDtitletxt').text();
        frameTitle = frameTitle.toLowerCase();
        // is override message alert window
        if (frameTitle.indexOf('override') !== -1) {
            sendPost('STDCIF1', 'Check', 'Maker', 3); // end check

            $(this.contentDocument).find('table#TBLPageTAB_FOOTER input#BTN_ACCEPT').on('mousedown', function() {
                sendPost('STDCIF1', 'Accept to Proceed', 'Maker', 4); // start save
            });
        }
        // is information message alert window
        if (frameTitle.indexOf('information') !== -1) {
            sendPost('STDCIF1', 'Save successfully', 'Maker', 5); // end save

            $(this.contentDocument).find('table#TBLPageTAB_FOOTER input#BTN_OK').on('click', function() {
                sendPost('STDCIF1', 'OK', 'Maker', 6);
            });
        }
    });
});

// Print the_chu_ky
$(document).on('click', '#TAB_HEADER__SEC_2__PART button#BLK_CUSTOMER__UIBTN_PRINT_SIGN_CARD', function() {
    sendPost('STDCIF1', 'Print the_chu_ky', 'Maker', 7, true);
});

// Print the_TK
$(document).on('click', '#TAB_HEADER__SEC_2__PART button#BLK_CUSTOMER__UIBTN_PRINT_CARD_ACCOUNT', function() {
    sendPost('STDCIF1', 'Print the_TK', 'Maker', 7, true);
});

// Print form_TK_song_ngu
$(document).on('click', '#TAB_HEADER__SEC_2__PART button#BLK_CUSTOMER__LBL_BTN_IN_FORM_TKSN', function() {
    sendPost('STDCIF1', 'Print the_TK_song_ngu', 'Maker', 7, true);
});

// Print phieu_nhan_the
$(document).on('click', '#TAB_HEADER__SEC_2__PART button#BLK_CUSTOMER__BTN_PRINT_ISSUE_CARD', function() {
    sendPost('STDCIF1', 'Print phieu_nhan_the', 'Maker', 7, true);
});

// Print form_TK_TV
$(document).on('click', '#TAB_HEADER__SEC_2__PART button#BLK_CUSTOMER__UIBTN_PRINT_FORM', function() {
    sendPost('STDCIF1', 'Print form_TK_TV', 'Maker', 7, true);
});

/**
 * Step 2: Checker
 */
/** Method 1: Search by Customer No */
$(document).on(
    'mouseenter',
    'iframe[id^=ifr_LaunchWin][src*="funcid=STSCIF1"]',
    function() {
        if (!localStorage.getItem(getCurrentFormContext())) {
            sendPost('STSCIF1', 'Start search by Customer No', 'Checker', 1); // start search by customer no
            localStorage.setItem(getCurrentFormContext(), 1); // set this form as already opened
        }
    }
);
// when close current STSCIF1 form, remove it from localStorage for better storage management
$(document).on('mouseup', 'a#WNDbuttons[title="Close"]', function() {
    localStorage.removeItem(getCurrentFormContext());
});

// hit search
$(document).on('click', 'li#Search.BTNIconExecuteQuery > a.TBitem', function() {
    sendPost('STSCIF1', 'Start querying', 'Checker', 2); // start querying
});
// double-click on record in result table
$(document).on(
    'dblclick',
    'div.WNDcontainer div.WNDcontent.mediumwin div#ResTree div#QryRslts table#TBL_QryRslts > tbody > tr',
    function() {
        localStorage.setItem("searchByCustomerNo", 1); // save action, user is opening a customer record
    }
);
// STDCIF1 form shows up, opening a customer record
$(document).on(
    'mouseenter',
    'iframe[id^=ifr_LaunchWin][src*="funcid=STDCIF1"]',
    function() {
        let isSearchByCustomerNo = localStorage.getItem("searchByCustomerNo");
        if (isSearchByCustomerNo === "1") {
            sendPost('STDCIF1', 'Finish search', 'Checker', 3); // end search
            localStorage.setItem("searchByCustomerNo", 0); // complete search by customer number, clear action
        }
    }
);

/** Method 2: Search by CIF */
$(document).on('click', 'li#EnterQuery > a.TBitem', function() {
    sendPost('STDCIF1', 'Start Enter query', 'Checker', 1); // start enter query
});
// execute query
$(document).on('click', 'li#ExecuteQuery > a.TBitem', function() {
    // update current cif
    let cifInput = $('input[name="CUSTNO"]');
    let cifVal = $(cifInput).val();
    localStorage.setItem("cif", cifVal);

    sendPost('STDCIF1', 'Start execute query', 'Checker', 2); // end enter query, start executing query

    /** TODO: Dummy data - cannot catch actual end of query execution */
    sendPost('STDCIF1', 'End execute query', 'Checker', 3); // end execute query
    /** */
});

// Authorize
$(document).on('click', 'li#Authorize > a.TBitem', function() {
    sendPost('STDCIF1', 'Authorize', 'Checker', 4); // start add new

    let currFrameBody = $(this).parents('body');
    let authIfr = $(currFrameBody).find('iframe#ifrSubScreen');

    $(authIfr).on('load', function() {
        sendPost('STDCIF1', 'Open Authorize form', 'Checker', 5); // open authorize form

        $(this.contentDocument).find('table#TBLPageTAB_FOOTER input#BTN_OK').on('mousedown', function(el) {
            sendPost('STDCIF1', 'Accept to Authorize', 'Checker', 6); // start authorizing

            let currFrameBody = $(el.currentTarget).parents('body');
            let alertIfr = $(currFrameBody).find('#ifr_AlertWin');

            $(alertIfr).on('load', function() {
                let frameTitle = $(this.contentDocument).find('h1.WNDtitletxt').text();
                frameTitle = frameTitle.toLowerCase();
                if (frameTitle.indexOf('information') !== -1) {
                    sendPost('STDCIF1', 'Authorize successfully', 'Checker', 7, true); // end authorizing
                }
            });
        });
    });
});

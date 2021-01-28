<?php

/**
 * CONSTANTS
 */
const DEFAULT_CSV_FILE_NAME = "data.csv";

const COLUMNS = array(
    "browser",
    "url",
    "branch",
    "user_name",
    "Actor",
    "form_name",
    "id",
    "cif",
    "process_name",
    "step",
    "start_time",
    "end_time",
    "duration"
);
const BROWSER_NAME = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0";
const URL = "http://10.1.28.156:7003/FCJNeoWeb/SMSStartLogServlet";
const BRANCH = "'065";
const ACTOR = "Maker";
const FORM_NAME = "STDCIF1";
const STEPS = [
    1 => "Add new",
    2 => "Save",
    3 => "Check",
    4 => "Accept to Proceed",
    5 => "Save successfully",
    6 => "OK",
    7 => "Print"
];
const STEP_DURATION_RANGE = [
    1 => ['min' => 50000, 'max' => 150000],
    2 => ['min' => 10, 'max' => 5000],
    3 => ['min' => 10, 'max' => 2000],
    4 => ['min' => 10, 'max' => 5000],
    5 => ['min' => 10, 'max' => 2000],
    6 => ['min' => 10, 'max' => 5000],
    7 => ['min' => 0, 'max' => 0]
];
const PRINT_TYPES = ["the_chu_ky", "the_TK", "form_TK_TV"];
const USERS = ["TEST01", "TEST02"];
const ID_RANGE = [
    "min" => 100000000,
    "max" => 999999999
];
const CIF_RANGE = [
    "min" => 1000000,
    "max" => 9999999
];
const DATE_RANGE = [
    "18 January 2021",
    "19 January 2021",
    "20 January 2021",
    "21 January 2021",
    "22 January 2021"
];
const TIME_RANGE = [
    [8, 11],
    [13, 16]
];
const MAX_RECORD = 10000;

/**
 * INITIATION
 */
/** Get file name */
$fileName = readline("Enter data file name (default: " . DEFAULT_CSV_FILE_NAME . "): ");
if (!$fileName) {
    $fileName = DEFAULT_CSV_FILE_NAME;
}

/** Get write mode */
do {
    $writeMode = readline("Choose write mode [a, w] (default: w): ");
    if (!$writeMode) {
        $writeMode = 'w';
    }
}
while (!in_array($writeMode, ['a', 'w'], true));

/** Get number of records (processes) */
do {
    $limit = readline("Number of Processes (0 < x <= " . MAX_RECORD . "): ");
}
while ($limit <= 0 || $limit > MAX_RECORD);

/** Open and write columns */
$fp = fopen($fileName, $writeMode);
fputcsv($fp, COLUMNS);

/**
 * DATA GENERATION
 */
for ($i = 0; $i < $limit; $i++) {
    // random data for whole process
    $currentUser = rand(0, count(USERS)-1);
    $currentPrint = rand(0, count(PRINT_TYPES)-1);
    $currentId = rand(ID_RANGE['min'], ID_RANGE['max']);
    $currentCif = rand(CIF_RANGE['min'], CIF_RANGE['max']);
    $currentDate = DATE_RANGE[rand(0, count(DATE_RANGE)-1)];
    $randomHours = array(
        rand(TIME_RANGE[0][0], TIME_RANGE[0][1]),
        rand(TIME_RANGE[1][0], TIME_RANGE[1][1])
    );
    $h = $randomHours[rand(0, 1)]; // get a random hour in working time
    $initStartTime = $h . ":" . rand(0, 59) . ":" . rand(0, 59);
    $initStartDateTime = $currentDate . " " . $initStartTime;
    $initStartTimestamp = strtotime($initStartDateTime);

    // prepare some vars
    $lastStepEndTimestamp = "";

    // generating whole process
    foreach (STEPS as $step => $processName) {
        $row = array();

        // prepare time data
        $currentStartTimestamp = ($step === 1) ? $initStartTimestamp : $lastStepEndTimestamp;
        $currentDuration = rand(STEP_DURATION_RANGE[$step]['min'], STEP_DURATION_RANGE[$step]['max']);
        $currentEndTimestamp = $currentStartTimestamp + $currentDuration;
        $lastStepEndTimestamp = $currentEndTimestamp;

        // assigning data
        // browser
        $row[] = BROWSER_NAME;
        // url
        $row[] = URL;
        // branch
        $row[] = BRANCH;
        // user_name
        $row[] = USERS[$currentUser];
        // Actor
        $row[] = ACTOR;
        // form_name
        $row[] = FORM_NAME;
        // id
        $row[] = $currentId;
        // cif
        $row[] = $currentCif;
        // process_name
        $row[] = ($step !== 7) ? $processName : $processName . " " . PRINT_TYPES[$currentPrint];
        // step
        $row[] = $step;
        // start_time
        $row[] = $currentStartTimestamp;
        // end_time
        $row[] = ($step !== 7) ? $currentEndTimestamp : "N/A";
        // duration
        $row[] = $currentDuration;

        // data row ready!
        // writing to file
        fputcsv($fp, $row);
    }
}

/** END */
fclose($fp);

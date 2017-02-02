export class Course {
    Name: string;
    Title: string;
    Type: string;
}

export class Category {
    Name: string;
    Title: string;
    Description: string;
    Examples: Example[];
}

export class Example {
    Name: string;
    Title: string;
    Course: string;
    Type: string;
    Category: string;
    Description: string;
    Difficulty: string;
    Requires: string[];
    SourceCode: string;
    UseThisMain: string;
    Instruction: string;
    TestFiles: TestFile[];

    SessionID: string;
    StartTime: Date;
}

export class TestFile {
    Name: string;
    In: string;
    SExp: string;
}

export class Test {
    Course: string;
    Example: string;
    SessionID: string;
    StartTime: Date;
    Code: string;
    CompileAndTest: boolean;

    CompileOutput: string;
    CompileFailed: boolean;
    TestOutput: string;
    TestFinished: boolean;

    NumOfTests: number = 0;
    NumOfSucceeded: number = 0;
    Succeeded: boolean = false;
}

export class User {
    UID: string;
    Name: string;
    Password: string;
    Jwt: string;
    Roles: string[];
    IsAuthenticated: boolean;
    IsTeacher: boolean;

    Results: ExampleResult[];
}

export class ExampleResult {
    ID: number;

    Course: string;
    CourseTitle: string;
    Example: string;
    ExampleTitle: string;

    FirstAttempt: Date;
    LastAttempt: Date;
    Time: number;
    NumOfAttempts: number;

    NumOfTests: number;
    NumOfSucceeded: number;
    NumOfFailed: number;
    NumOfErrors: number;
    NumOfSkipped: number;

    NumOfCompilations: number;
    NumOfTestRuns: number;

    LinesOfCode: number;
    CyclomaticComplexity: number;
    MemErrors: number;
}
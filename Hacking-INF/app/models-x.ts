export class Course {
    Name: string;
    Title: string;
    Type: string;
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
    Instruction: string;
    SessionID: string;
    StartTime: Date;
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
}

export class User {
    UID: string;
    Name: string;
    Password: string;
    Roles: string[];
    IsAuthenticated: boolean;
    IsTeacher: boolean;
}
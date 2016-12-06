export class Course {
    Name: string;
    Title: string;
}

export class Example {
    Name: string;
    Title: string;
    Course: string;
    Category: string;
    Description: string;
    Difficulty: string;
    Requires: string[];
    SourceCode: string;
    SessionID: string;
    Instruction: string;
}

export class Test {
    Course: string;
    Example: string;
    SessionID: string;
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
}
export class BasicStat {
    NumOfSucceeded: number = 0;
    NumOfTests: number = 0;
}

export class Course {
    Name: string;
    Title: string;
    Type: string;

    Stat: BasicStat = new BasicStat();
}

export class Category {
    Name: string;
    Title: string;
    Description: string;
    Examples: Example[];

    Stat: BasicStat = new BasicStat();
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

    Result: ExampleResult;
}

export class TestFile {
    Name: string;
    In: string;
    SExp: string;
}

export class CompilerMessage {
    Message: string;
    Hint: string;
}

export class CompilerMessageHint {
    Message: string;
    Hint: string;
}

export class ReportedCompilerMessage {
    Course: string;
    CourseTitle: string;
    Example: string;
    ExampleTitle: string;

    UID: string;
    Date: Date;

    Message: string;
    Hint: string;
}

export class Test {
    Course: string;
    Example: string;
    SessionID: string;
    StartTime: Date;
    Code: string;
    CompileAndTest: boolean;

    CompileOutput: string;
    CompileOutputFormatted: string;
    CompileFailed: boolean;
    TestOutput: string;
    MemoryErrors: MemoryErrors[] = [];
    TestFinished: boolean;

    NumOfTests: number = 0;
    NumOfSucceeded: number = 0;
    Succeeded: boolean = false;
}

export class MemoryErrors {
    TestCase: string;
    Report: string;
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

export class ExampleStat {

    Course: string;
    CourseTitle: string;
    Example: string;
    ExampleTitle: string;

    FirstAttempt: Date;
    LastAttempt: Date;
    AvgTime: string;
    MedTime: string;
    StdDevTime: number;
    NumOfAttempts: number;

    NumOfTests: number;
    AvgNumOfSucceeded: number;
    AvgNumOfTestRuns: number;
}

export class StudentStat {
    User: string;
    UID: string;

    Details: StudentStatDetail[];
}

export class StudentStatDetail {
    Course: string;
    CourseTitle: string;
    Example: string;
    ExampleTitle: string;

    FirstAttempt: Date;
    LastAttempt: Date;
    Time: string;

    NumOfTests: number;
    NumOfSucceeded: number;
    NumOfTestRuns: number;
}

export class LogLineModel {
    Message: string;
    Color: string;
}
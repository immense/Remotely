export interface Result<T = void> {
	HadException: boolean;
	IsSuccess: boolean;
	Reason: string;
	Value?: T;
}
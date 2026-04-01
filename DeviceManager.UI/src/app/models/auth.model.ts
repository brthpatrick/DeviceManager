export interface RegisterRequest {
    name: string;
    email: string;
    password: string;
    role: string;
    location: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface AuthResponse {
    token: string;
    userId: number;
    name: string;
    email: string;
}